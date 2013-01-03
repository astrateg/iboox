using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Recaptcha;
using iBoox.Models;

namespace iBoox.Controllers
{
	public class AccountController : Controller
	{
		public ViewResult LogOn()
		{
			return View();
		}

		[HttpPost]
		public ActionResult LogOn(LogOnModel model, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				if (Membership.ValidateUser(model.UserName, model.Password))
				{
					FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
					if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
						&& !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
					{
						return Redirect(returnUrl);
					}
					else
					{
						return RedirectToAction("Index", "Book");
					}
				}
				else
				{
					ModelState.AddModelError("", "Логин или пароль некорректны");
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		public ActionResult LogOff()
		{
			FormsAuthentication.SignOut();

			return RedirectToAction("Index", "Book");
		}

		public ViewResult Register()
		{
			return View();
		}

		[HttpPost]
		[RecaptchaControlMvc.CaptchaValidator]
		public ActionResult Register(RegisterModel model, bool? captchaValid, string captchaErrorMessage)
		{
			if (ModelState.IsValid)
			{
				if (captchaValid ?? false)
				{
					// Attempt to register the user
					MembershipCreateStatus createStatus;
					Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

					if (createStatus == MembershipCreateStatus.Success)
					{
						FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
						return RedirectToAction("Index", "Book");
					}
					else
						ModelState.AddModelError("", ErrorCodeToString(createStatus));
				}
				else
					ModelState.AddModelError("captcha", captchaErrorMessage);
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[Authorize]
		public ViewResult ChangePassword(string resetPassword)
		{
			ChangePasswordModel model = new ChangePasswordModel();
			if (!String.IsNullOrEmpty(resetPassword))
				model.OldPassword = resetPassword;

			return View(model);
		}

		[Authorize]
		[HttpPost]
		public ActionResult ChangePassword(ChangePasswordModel model)
		{
			if (ModelState.IsValid)
			{

				// ChangePassword will throw an exception rather than return false in certain failure scenarios.
				bool changePasswordSucceeded;
				try
				{
					MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
					changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
				}
				catch (Exception)
				{
					changePasswordSucceeded = false;
				}

				if (changePasswordSucceeded)
				{
					TempData["message"] = "Пароль был успешно изменен.";
					return RedirectToAction("Index", "Book");   // Возврат на стартовую страницу
				}
				else
				{
                    ModelState.AddModelError("", "Текущий пароль неверен или новый пароль некорректен.");
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}




		public ActionResult ResetPassword(string reset, string user)
		{
			if (reset != null && user != null)
			{
				var currentUser = Membership.GetUser(user);
				if (currentUser != null)
				{
					if (Helpers.Utils.HashResetParams(currentUser.UserName, currentUser.ProviderUserKey.ToString()) == reset)
					{
						ViewBag.NewPassword = currentUser.ResetPassword();
						ViewBag.UserName = currentUser.UserName;
						return View("NewPassword");
					}
				}
			}

			return View();
		}

		[HttpPost]
		public ActionResult ResetPassword(ResetPasswordModel model)
		{
			string userByEmail = Membership.GetUserNameByEmail(model.Email);
			if (userByEmail == null)
			{
				ModelState.AddModelError("", "E-mail некорректен");
				return View(model);
			}

			MembershipUser currentUser = Membership.GetUser(model.UserName);
			if (currentUser == null)
			{
				ModelState.AddModelError("", "Логин некорректен");
				return View(model);
			}

			if (userByEmail == currentUser.UserName)
			{
				var hostUri = Request.Url.GetLeftPart(UriPartial.Authority);

				try
				{
					Helpers.Utils.SendResetEmail(currentUser, (hostUri != String.Empty ? hostUri : "http://www.example.com"));
					TempData["Message"] = "Письмо-подтверждение отправлено на указанный e-mail.";
					return RedirectToAction("Index", "Book");
				}
				catch (Exception)
				{
					return View();
				}
			}
			else
			{
				TempData["Message"] = String.Format("Имя пользователя \"{0}\" не соответствует e-mail \"{1}\"", model.UserName, model.Email);
				return RedirectToAction("ResetPassword");
			}
		}


		//[Authorize]
		//public ViewResult ChangeQuestion()
		//{
		//    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true);
		//    ChangeQuestionModel model = new ChangeQuestionModel { 
		//        SecretQuestion = currentUser.PasswordQuestion
		//    };
		//    return View(model);
		//}

		//[Authorize]
		//[HttpPost]
		//public ActionResult ChangeQuestion(ChangeQuestionModel model)
		//{
		//    if (ModelState.IsValid)
		//    {
		//        // ChangeQuestion will throw an exception rather than return false in certain failure scenarios.
		//        bool changeQuestionSucceeded;
		//        try
		//        {
		//            MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
		//            changeQuestionSucceeded = currentUser.ChangePasswordQuestionAndAnswer(model.Password, model.SecretQuestion, model.SecretAnswer);
		//        }
		//        catch (Exception)
		//        {
		//            changeQuestionSucceeded = false;
		//        }

		//        if (changeQuestionSucceeded) {
		//            TempData["message"] = "Your secret question has been changed successfully.";
		//            return RedirectToAction("Index", "Book");   // Возврат на стартовую страницу
		//        }
		//        else
		//            ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
		//    }

		//    // If we got this far, something failed, redisplay form
		//    return View(model);
		//}

		//public ViewResult ForgotPassword()
		//{
		//    return View(new RegisterModel());
		//}

		//[HttpPost]
		//public ActionResult ForgotPassword(RegisterModel model)
		//{
		//    // Этот метод обрабатывает события сразу от двух кнопок
		//    if (!String.IsNullOrEmpty(model.SecretAnswer))
		//    {
		//        string resetPassword = Membership.Provider.ResetPassword(model.UserName, model.SecretAnswer);
		//        if (Membership.ValidateUser(model.UserName, resetPassword))
		//        {
		//            FormsAuthentication.SetAuthCookie(model.UserName, false);
		//            return RedirectToAction("ChangePassword", new { resetPassword = resetPassword });
		//        }
		//        else
		//        {
		//            TempData["Message"] = "Invalid Response";
		//            return View();
		//        }
		//    }

		//    MembershipUser user = Membership.GetUser(model.UserName, false);
		//    RegisterModel newModel = new RegisterModel();
		//    if (user != null)
		//    {
		//        newModel.SecretQuestion = user.PasswordQuestion;
		//        newModel.UserName = model.UserName;
		//    }
		//    else
		//    {
		//        TempData["ErrorMessage"] = "The user you have specified is invalid, please recheck your username and try again";
		//        newModel.SecretQuestion = String.Empty;
		//    }

		//    return View(newModel);
        //}

        [Authorize(Roles = "Admin")]
        public ViewResult ManageAccounts(string searchType, string searchInput)
        {
            List<SelectListItem> searchOptionList = new List<SelectListItem>() 
            {
                new SelectListItem() { Value = "UserName", Text = "UserName" },
                new SelectListItem() { Value = "Email", Text = "Email" }
            };

            MembershipUserCollection accounts;

            if (String.IsNullOrEmpty(searchInput))
                accounts = Membership.GetAllUsers();
            else if (searchType == "Email")
                accounts = Membership.FindUsersByEmail(searchInput);
            else
                accounts = Membership.FindUsersByName(searchInput);

            AccountsListViewModel viewModel = new AccountsListViewModel
            {
                SearchOptionList = new SelectList(searchOptionList, "Value", "Text", searchType ?? "UserName"),
                SearchInput = searchInput ?? string.Empty,
                Accounts = accounts
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public JsonResult DeleteAccount(string id)
        {
            Membership.DeleteUser(id);
            return Json(new { id = id });
        }

        [Authorize(Roles = "Admin")]
        public ViewResult EditAccount(string id)
        {
            AccountViewModel viewModel = new AccountViewModel();
            viewModel.Account = Membership.GetUser(id);

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ViewResult EditAccount(string id, bool approved)
        {
            // Is a list of all the user roles
            List<String> removeRoleList = new List<String>(Roles.GetAllRoles());

            // We are requesting the form variables directly from the form
            foreach (string key in Request.Form.Keys)
            {
                if (key.StartsWith("role."))
                {
                    String userRole = key.Substring(5, key.Length - 5);

                    // ВСЕ чекбоксы передаются в Request.Form, даже неотмеченные!!!
                    // Поэтому проверяем содержимое "ролевых" чекбоксов (если не отмечен: "false" | если отмечен: "true;false" - такая хитрость из-за скрытых полей)
                    bool isRoleChecked = (Request.Form[key].Contains("true")) ? true : false;
                    if (isRoleChecked)
                        removeRoleList.Remove(userRole);

                    if (!Roles.IsUserInRole(id, userRole))
                        Roles.AddUserToRole(id, userRole); // а если у юзера уже есть эта роль, нет смысла ее повторно назначать
                }
            }

            foreach (string removeRole in removeRoleList)
            {
                Roles.RemoveUserFromRole(id, removeRole);
            }

            MembershipUser account = Membership.GetUser(id);
            account.IsApproved = approved;
            Membership.UpdateUser(account);
            AccountViewModel viewModel = new AccountViewModel();
            viewModel.Account = account;
            
            TempData["Message"] = "Данные аккаунта обновлены";
            return View(viewModel);
        }

        #region Manage Roles
        [Authorize(Roles = "Admin")]
		public ViewResult ManageRoles()
		{
			//ViewBag.TotalRoles = Roles.GetAllRoles().Count();
			return View();
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public ActionResult CreateRole(string newRole)
		{
			Roles.CreateRole(newRole);
			return RedirectToAction("ManageRoles");
		}

		[Authorize(Roles = "Admin")]
		public JsonResult DeleteRole(string id)
		{
			Roles.DeleteRole(id);
			return Json(new { id = id, TotalRoles = Roles.GetAllRoles().Count() });
		}
        #endregion



        #region Status Codes
        [NonAction]
		private static string ErrorCodeToString(MembershipCreateStatus createStatus)
		{
			// See http://go.microsoft.com/fwlink/?LinkID=177550 for a full list of status codes.
			switch (createStatus)
			{
				case MembershipCreateStatus.DuplicateUserName:
                    return "Логин уже существует. Пожалуйста, введите другой логин.";

				case MembershipCreateStatus.DuplicateEmail:
                    return "Логин для этого e-mail уже существует. Пожалуйста, введите другой e-mail.";

				case MembershipCreateStatus.InvalidPassword:
                    return "Введенный пароль некорректен. Пожалуйста, введите корректный пароль.";

				case MembershipCreateStatus.InvalidEmail:
                    return "Введенный e-mail некорректен. Пожалуйста, введите корректный e-mail.";

				case MembershipCreateStatus.InvalidUserName:
                    return "Введенный логин некорректен. Пожалуйста, введите корректный логин.";

				case MembershipCreateStatus.ProviderError:
					return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				case MembershipCreateStatus.UserRejected:
					return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				default:
					return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
			}
		}
		#endregion
	}
}
