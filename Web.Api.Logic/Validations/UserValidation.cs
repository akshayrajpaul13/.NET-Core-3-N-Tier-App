using System;
using System.Linq;
using Web.Api.Base.Extensions;
using Web.Api.Base.Validations;
using Web.Api.Data.Enums;
using Web.Api.Data.Helpers.Repositories;
using Web.Api.Data.Models;
using Web.Api.Logic.Exceptions;
using Web.Api.Logic.Services;
using Web.Api.Logic.Validations.Helpers;

namespace Web.Api.Logic.Validations
{
    public class UserValidation : BaseValidationService, IUserValidation
    {
        private readonly ISecurityService _securityService;

        public UserValidation(IUnitOfWork unitOfWork, ISecurityService securityService) : base(unitOfWork)
        {
            _securityService = securityService;
        }

        /// <summary>
        /// Perform model-level validation of a user
        /// </summary>
        public void Validate(User user, string password)
        {
            ValidateModel(user, password);
            ValidateBusinessRules(user);
        }

        private void ValidateModel(User user, string password)
        {
            // validate email
            if (!EmailValidation.IsEmailFormatValid(user.Email))
                throw new ValidationException($"Email '{user.Email}' is not valid");

            //// student must have tenant
            //if (user.RoleId.EqualsEnum(Roles.Student) && user.TenantId.IsNotSet())
            //    throw new ValidationException("Tenant is required");

            // check that a valid role is provided
            if (!user.RoleId.IsInEnum<Roles>() || user.RoleId.ToEnum<Roles>() == Roles.Undefined)
                throw new ValidationException("Role is required");

            // validate login type
            Validator.Required(user.LoginTypeId, "Login Type");

            if (user.LoginTypeId.EqualsEnum(LoginTypes.Default))
            {
                // check password for new users
                if (user.Id.IsNotSet() || !string.IsNullOrWhiteSpace(password))
                    ValidateNewPassword(user, password);
            }
            else
            {
                // check social media fields set
                ValidateForSocialMediaLogin(user);
            }
        }

        /// <summary>
        /// Perform model-level validation of a user when using Social Media Login
        /// </summary>
        private void ValidateForSocialMediaLogin(User user)
        {
            //TODO ES 2017-11-06: when are the facebookid fields etc being set?
        }

        /// <summary>
        /// Validate that a user's password is strong enough according to the rules of the tenant
        /// </summary>
        public void ValidateNewPassword(User user, string password)
        {
            // Validate strength
            var result = _securityService.IsPasswordStrong(null, password);
            if (!result.Successful)
                throw new ValidationException($"Password does not meet requirements: {result.Message}");

            // Validate password hasn't been used before
        }

        private void ValidateBusinessRules(User user)
        {
            // validate user email not exists
            // currently, students can reuse emails across different tenants, but web admin users can't until the login process has been reworked
            if (UnitOfWork.Users.GetAll(x => x.Email == user.Email && x.Id != user.Id).Any())
                throw new Exception("A user with that email already exists");
        }

        public void ValidateCanBeDeleted(User user)
        {
        }

        public void ValidateRoleCanLogin(Roles role)
        {
            if (role.IsOneOf(Roles.Undefined))
                throw new ValidationException("You do not have the required permissions to access this site");
        }

        public void ValidateLoginTypeCanLogin(User user)
        {
            if (user.LoginTypeId.NotEqualsEnum(LoginTypes.Default))
            {
                var loginType = user.LoginTypeId.ToEnumDisplayString<LoginTypes>();
                throw new Exception($"The account with this email address was created using a {loginType} login. Please click the {loginType} button to sign-in.");
            }
        }
    }
}
