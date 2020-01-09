using System;
using System.Transactions;
using Web.Api.Base.Extensions;
using Web.Api.Data.Enums;
using Web.Api.Data.Helpers.Repositories;
using Web.Api.Data.Models;
using Web.Api.Logic.Models.Account;
using Web.Api.Logic.Validations;

namespace Web.Api.Logic.Services
{
    public sealed class UserService : BaseService, IUserService
    {
        private readonly IUserValidation _userValidationService;
        private readonly ISecurityService _securityService;
        private readonly IPermissionService _permissionService;
        private readonly SessionService _sessionService;

        public UserService(IUnitOfWork unitOfWork, 
            IUserValidation userValidationService, 
            ISecurityService securityService,
            IPermissionService permissionService,
            ISessionService sessionService) : base(unitOfWork)
        {
            _userValidationService = userValidationService;
            _securityService = securityService;
            _permissionService = permissionService;
            _sessionService = sessionService as SessionService;
        }

        /// <summary>
        ///     Creates a new User and UserProfile, with permissions as defined by the user's role
        /// </summary>
        public User Create(User user, string password)
        {
            using (var transaction = new TransactionScope())
            {
                if (user.LoginTypeId == LoginTypes.Default.ToInt())
                    SetNewUserDefaults(user, password);
                else
                    SetSocialLoginUserDefaults(user);

                // validate
                _userValidationService.Validate(user, password);

                // create User, UserProfile, permissions
                user = Uow.Users.Create(user);
                _permissionService.ResetUserPermissionsToRoleDefaults(user.Id);

                if (user.LoginTypeId == LoginTypes.Default.ToInt()) user.AuthenticationToken = null;

                transaction.Complete();

                return user;
            }
        }

        private void SetNewUserDefaults(User user, string password)
        {
            var confirmEmailExpiryLimitInHours = 1;

            user.PasswordHash = _securityService.HashPassword(password);
            user.ConfirmEmailExpiry = DateTime.UtcNow.AddHours(confirmEmailExpiryLimitInHours);
            user.AccessFailedCount = 0;
            user.LockoutEndDateUtc = null;
            user.VerificationCode = VerificationCodeGenerator.GenerateVerificationCode(Uow, user);
        }

        private void SetSocialLoginUserDefaults(User user)
        {
            user.PasswordHash = null;
            user.ConfirmEmailExpiry = null;
            user.AccessFailedCount = 0;
            user.LockoutEndDateUtc = null;
            user.VerificationCode = null;
        }

        #region Auth
        /// <summary>
        ///     Logs a User in and creates a Session if the User password is successfully validated
        /// </summary>
        public LoginResult Login(User user, string password, int loginSource, bool createSession = true)
        {
            _userValidationService.ValidateRoleCanLogin(user.RoleId.ToEnum<Roles>());
            _userValidationService.ValidateLoginTypeCanLogin(user);

            ValidateUserNotLockedOut(user);

            if (!ValidatePassword(user, password))
            {
                UpdateAccessFailureAndLockout(user, 10, 5);
                throw new Exception("Invalid Email or Password specified");
            }

            // Set failure count to zero again
            ResetAccessFailure(user);

            // return active Session
            Session session = null;
            if (createSession)
                session = _sessionService.GetOrCreateSession(user.Id, loginSource);

            return LoginResult.Get(session?.SessionGuid, user.ForcePasswordReset, !user.EmailConfirmed);
        }

        /// <summary>
        ///     Logs a User out of the current Session
        /// </summary>
        /// <param name="sessionGuid">string</param>
        public void Logout(string sessionGuid)
        {
            _sessionService.EndSession(sessionGuid);
        }

        /// <summary>
        ///     Validates the User's password
        /// </summary>
        /// <param name="user">User object</param>
        /// <param name="password">string detailing the User password</param>
        /// <returns>bool</returns>
        private bool ValidatePassword(User user, string password)
        {
            return _securityService.VerifyHashedPassword(user.PasswordHash, password);
        }

        private void ValidateUserNotLockedOut(User user)
        {
            if (user.LockoutEndDateUtc != null && user.LockoutEndDateUtc > DateTime.UtcNow)
            {
                if (user.LockoutEndDateUtc.Value.Date != DateTime.MaxValue.Date)
                    throw new Exception("Account is temporarily locked");

                throw new Exception("Account is locked");
            }
        }

        /// <summary>
        /// Checks if the User Account should be locked based on the number of failed login attempts
        /// </summary>
        private void UpdateAccessFailureAndLockout(User user, double failedLoginTimeoutInMinutes, int accessFailedCountLimit)
        {
            // if user failed login attempts exceed the limit prevent logins for a time period
            if (user.AccessFailedCount > accessFailedCountLimit)
            {
                user.AccessFailedCount = 0;
                user.LockoutEndDateUtc = DateTime.UtcNow.AddMinutes(failedLoginTimeoutInMinutes);
                Uow.Users.Update(user);

                throw new Exception("Account is temporarily locked");
            }

            // update user failed login attempts
            user.AccessFailedCount++;

            Uow.Users.Update(user);
        }

        /// <summary>
        /// Resets the access failure count to zero
        /// </summary>
        private void ResetAccessFailure(User user)
        {
            if (user.AccessFailedCount > 0)
            {
                user.AccessFailedCount = 0;
                Uow.Users.Update(user);
            }
        }
        #endregion
    }
}
