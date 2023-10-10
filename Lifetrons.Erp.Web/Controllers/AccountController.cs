using Lifetrons.Erp.Helpers;
using Lifetrons.Erp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Practices.Unity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Task = System.Threading.Tasks.Task;

namespace Lifetrons.Erp.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {

        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult LoginPartial()
        {
            return PartialView("_LoginPartial");
        }

        public ApplicationUser GetUserByName(string userName)
        {
            ApplicationUser user = UserManager.FindByName(userName);
            return user;
        }

        public ApplicationUser GetUserById(string userId)
        {
            ApplicationUser user = UserManager.FindById(userId);
            return user;
        }

        public async Task<ApplicationUser> UpdateUserOrganization(string userId, string orgId)
        {
            //Update user organization
            ApplicationUser user = UserManager.FindById(userId);
            user.OrgId = Guid.Parse(orgId);
            user.Active = true;
            await UserManager.UpdateAsync(user);

            ////Assign roles
            //AddUserToRole(user.Id, "StockAuthorize");
            //AddUserToRole(user.Id, "StockEdit");

            return user;
        }

        public bool AddUserToRole(string userId, string roleName)
        {
            ApplicationUser user = UserManager.FindById(userId);
            var idResult = UserManager.AddToRole(userId, roleName);
            return idResult.Succeeded;
        }

        public bool RemoveUserToRole(string userId, string roleName)
        {
            ApplicationUser user = UserManager.FindById(userId);
            var idResult = UserManager.RemoveFromRole(userId, roleName);
            return idResult.Succeeded;
        }

        [AllowAnonymous]
        public ActionResult LegalAgreement()
        {
            return View();
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = Url.Action("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost, ActionName("Login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = Url.Action("Index", "Home");
            }

            if (ModelState != null && ModelState.IsValid)
            {
                //var user = GetUserByName(model.UserName);
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    if (!user.Active)
                    {
                        return RedirectToAction("UserInactive", "Error");
                    }
                    await SignInAsync(user, model.RememberMe);

                    // Object Comparison and logging in audit
                    HttpContext.Items.Add(ControllerHelper.AuditData, "Logged In: User=" + user.Id + " User Name=" + user.UserName + " Email=" + user.AuthenticatedEmail + " Name=" + user.FirstName + " " + user.LastName);

                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", Resources.Resources.AccountController_Login_Invalid_username_or_password_);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            var model = new RegisterViewModel()
            {
                AddressLine1 = "Address Line 1",
                AddressLine2 = "Address Line 2",
                City = "Delhi",
                State = "Delhi",
                Country = "India",
                PostalCode = "110001",
            };

            return View(model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    UserName = model.UserName,
                    AuthenticatedEmail = model.AuthenticatedEmail,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    BirthDate = model.BirthDate,
                    Mobile = model.Mobile,
                    AddressLine1 = model.AddressLine1,
                    AddressLine2 = model.AddressLine2,
                    City = model.City,
                    State = model.State,
                    Country = model.Country,
                    PostalCode = model.PostalCode,
                    TimeZone = model.TimeZone,
                    Active = true
                };
                var result = await UserManager.CreateAsync(user,
                    model.Password);
                if (result.Succeeded)
                {
                    if (!user.Active)
                    {
                        return RedirectToAction("UserInactive", "Error");
                    }
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/Manage
        [HttpGet]
        [EsAuthorize(Roles = "SuperAdmin")]
        public async Task<ActionResult> ManageUserPassword(ManageMessageId? message)
        {
            var applicationUser = GetUserById(User.Identity.GetUserId());
            var container = Lifetrons.Erp.App_Start.UnityConfig.GetConfiguredContainer();
            //var contr = (Lifetrons.Erp.Logistics.Controllers.DispatchLineItemController) container.Resolve(typeof(Lifetrons.Erp.Logistics.Controllers.DispatchLineItemController), "DispatchLineItemController", Microsoft.Practices.Unity.ResolverOverride.);
            var aspNetUserService = container.Resolve<Lifetrons.Erp.Service.AspNetUserService>();
            ViewBag.UserId = new SelectList(await aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name");

            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";

            ViewBag.ReturnUrl = Url.Action("ManageUserPassword");
            return View();
        }

        // POST: /Account/Manage
        [HttpPost]
        [EsAuthorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ManageUserPassword([Bind(Include = "UserId,NewPassword,ConfirmPassword")]ManageUserPasswordViewModel model)
        {
            var applicationUser = GetUserById(User.Identity.GetUserId());
            var container = Lifetrons.Erp.App_Start.UnityConfig.GetConfiguredContainer();
            //var contr = (Lifetrons.Erp.Logistics.Controllers.DispatchLineItemController) container.Resolve(typeof(Lifetrons.Erp.Logistics.Controllers.DispatchLineItemController), "DispatchLineItemController", Microsoft.Practices.Unity.ResolverOverride.);
            var aspNetUserService = container.Resolve<Lifetrons.Erp.Service.AspNetUserService>();
            ViewBag.UserId = new SelectList(await aspNetUserService.SelectAsync(applicationUser.OrgId.ToString()), "Id", "Name", model.UserId);

            ModelState.Clear();

            if (TryValidateModel(model))
            {
                if (HasPassword(model.UserId))
                {
                    //Remove existing password
                    UserManager.RemovePassword(model.UserId);
                }
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                IdentityResult result = await UserManager.AddPasswordAsync(model.UserId, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("ManageUserPassword", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            if (ControllerContext.HttpContext.Session != null) ControllerContext.HttpContext.Session.RemoveAll();

            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { provider, returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> ExternalLoginCallback(string ReturnUrl)
        {
            //var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            var loginInfo = AuthenticationManager.GetExternalLoginInfo();
            if (loginInfo == null)
            {
                return View("NullResponseFromExternalLogin");
                //return RedirectToAction("Login");
            }

            //// Works for facebook and google
            //var externalIdentity = HttpContext.GetOwinContext().Authentication.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
            //var emailClaim = externalIdentity.Result.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            //var email = emailClaim.Value; 

            var externalIdentity = await AuthenticationManager
            .GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);

            var emailClaim = externalIdentity.Claims.FirstOrDefault(x =>
                x.Type.Equals(
                    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
                    StringComparison.OrdinalIgnoreCase));

            var emailAddressFromProvider = emailClaim != null
                ? emailClaim.Value
                : null;


            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                if (!user.Active)
                {
                    return RedirectToAction("UserInactive", "Error");
                }
                await SignInAsync(user, isPersistent: true);

                // Object Comparison and logging in audit
                HttpContext.Items.Add(ControllerHelper.AuditData, "Logged In: User=" + user.Id + " User Name=" + user.UserName + " Email=" + user.AuthenticatedEmail + " Name=" + user.FirstName + " " + user.LastName);

                return RedirectToLocal(ReturnUrl);
            }

            // If the user does not have an account, then prompt the user to create an account
            ViewBag.ReturnUrl = ReturnUrl;
            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
            var fName = loginInfo.ExternalIdentity.Name.Substring(0, loginInfo.ExternalIdentity.Name.IndexOf(" "));
            var lName = loginInfo.ExternalIdentity.Name.Substring(loginInfo.ExternalIdentity.Name.IndexOf(" "));
            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel
            {

                UserName = loginInfo.DefaultUserName,
                AuthenticatedEmail = emailAddressFromProvider,
                Email = emailAddressFromProvider,
                FirstName = fName,
                LastName = lName,
                AddressLine1 = "Address Line 1",
                AddressLine2 = "Address Line 2",
                City = "Delhi",
                State = "Delhi",
                Country = "India",
                PostalCode = "110001"
            });
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Audit(AuditingLevel = 1)]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }

                var user = new ApplicationUser()
                {
                    UserName = model.UserName,
                    AuthenticatedEmail = model.AuthenticatedEmail,
                    //PasswordHash = model.Password,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    BirthDate = model.BirthDate,
                    Mobile = model.Mobile,
                    TimeZone = model.TimeZone,
                    AddressLine1 = model.AddressLine1,
                    AddressLine2 = model.AddressLine2,
                    City = model.City,
                    State = model.State,
                    Country = model.Country,
                    PostalCode = model.PostalCode,
                    Active = true,
                };

                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        if (!user.Active)
                        {
                            return RedirectToAction("UserInactive", "Error");
                        }
                        await SignInAsync(user, isPersistent: false);

                        // Object Comparison and logging in audit
                        HttpContext.Items.Add(ControllerHelper.AuditData, "Logged In: User=" + user.Id + " User Name=" + user.UserName + " Email=" + user.AuthenticatedEmail + " Name=" + user.FirstName + " " + user.LastName);

                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }


        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        //public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await UserManager.FindByNameAsync(model.Email);
        //        if (user == null || !(await UserManager.IsEmailConfirmedAsync(user)))
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return View("ForgotPasswordConfirmation");
        //        }

        //        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
        //        // Send an email with this link
        //        var code = await UserManager.GeneratePasswordResetTokenAsync(user);
        //        var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Context.Request.Scheme);
        //        await emailService.SendMail(Data.Helper.SupportUserId, Data.Helper.SupportUserOrgId, model.Email, "", "Reset Password",
        //           "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
        //        return View("ForgotPasswordConfirmation");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            //var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            //AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

            await SetExternalProperties(identity);

            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);

            await SaveAccessToken(user, identity);

        }
        private async Task SetExternalProperties(ClaimsIdentity identity)
        {
            // get external claims captured in Startup.ConfigureAuth
            ClaimsIdentity ext = await AuthenticationManager.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);

            if (ext != null)
            {
                var ignoreClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims";
                // add external claims to identity
                foreach (var c in ext.Claims)
                {
                    if (!c.Type.StartsWith(ignoreClaim))
                        if (!identity.HasClaim(c.Type, c.Value))
                            identity.AddClaim(c);
                }
            }
        }

        private async Task SaveAccessToken(ApplicationUser user, ClaimsIdentity identity)
        {
            var userclaims = await UserManager.GetClaimsAsync(user.Id);

            foreach (var at in (
                from claims in identity.Claims
                where claims.Type.EndsWith("access_token")
                select new Claim(claims.Type, claims.Value, claims.ValueType, claims.Issuer)))
            {

                if (!userclaims.Contains(at))
                {
                    await UserManager.AddClaimAsync(user.Id, at);
                }
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPassword(string userId)
        {
            var user = UserManager.FindById(userId);
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }

                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}