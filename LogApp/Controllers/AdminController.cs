using AutoMapper;
using LogApp.Database.Configuration.Interfaces;
using LogApp.Database.Models;
using LogApp.Helpers;
using LogApp.Models;
using LogApp.Security.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LogApp.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private static readonly object _syncAddLogType = new object();
        private IRepository<User> userRepository;
        private IRepository<LogType> logTypeRepository;
        private IRepository<LogRecord> logRecordRepository;
        private IPasswordManager passwordManager;

        public AdminController(IRepository<User> userRepository,
            IRepository<LogType> logTypeRepository,
            IRepository<LogRecord> logRecordRepository,
            IPasswordManager passwordManager)
        {
            this.userRepository = userRepository;
            this.logTypeRepository = logTypeRepository;
            this.logRecordRepository = logRecordRepository;
            this.passwordManager = passwordManager;
        }
        [AllowAnonymous]
        public ActionResult Login(string errorMsg = null)
        {
            ViewData["ErrorMsg"] = string.IsNullOrEmpty(errorMsg) ? string.Empty : errorMsg;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            var user = await userRepository.EntityFindByAsync(u => u.Login == model.Login);
            if (user != null)
            {
                if (passwordManager.AreEqual(model.Password, user.Password, user.Salt))
                {
                    CreateCookie(user);
                    return RedirectToAction("Index", "Home");
                }
            }
            return Login("Nieprawidłowy login lub hasło");
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult AddLogType()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddLogType(LogTypeViewModel model)
        {
            if (model != null)
            {
                if (string.IsNullOrEmpty(model.Name))
                {
                    model.Name = Guid.NewGuid().ToString();
                }
                var constHeaders = new List<string> { "Czas zdarzenia", "Identyfikator źródła zdarzenia" };
                constHeaders.AddRange(model.Headers);
                model.Headers = constHeaders.ToArray();
                lock(_syncAddLogType)
                {
                    var logTypes = GetLogTypes();
                    bool exist = LogTypeExist(logTypes, model);
                    if (!exist)
                    {
                        var newType = new LogType(model.Name, model.Headers);
                        logTypeRepository.Add(newType);
                        logTypeRepository.Save();
                        ClearLogTypeCache();
                        model.SetSuccess("Nowy typ został dodany do bazy.");
                        return Json(model);
                    }
                    else
                    {
                        model.SetError("Typ o takich polach istnieje już w bazie.");
                        return Json(model);
                    }
                }
            }
            model = new LogTypeViewModel();
            model.SetError("Błędny model. Spróbuj ponownie później.");
            return Json(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult SearchLogType()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult LogTypeAjaxSource(jQueryDataTableParamModel param)
        {
            IEnumerable<string[]> result = null;
            int totalRecords = 0;
            var logTypes = GetLogTypes();
            var logTypesList = logTypes.ToList();
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                logTypesList = logTypesList.FindAll(x => LogTypeMach(x, param.sSearch));
            }
            switch (param.iSortCol_0)
            {
                case 0:
                    if (param.sSortDir_0 == "asc")
                    {
                        logTypesList.Sort((x, y) => Convert.ToInt32(x.Id).CompareTo(Convert.ToInt32(y.Id)));
                    }
                    else //desc
                    {
                        logTypesList.Sort((x, y) => Convert.ToInt32(y.Id).CompareTo(Convert.ToInt32(x.Id)));
                    }
                    break;
                case 1:
                    if (param.sSortDir_0 == "asc")
                    {
                        logTypesList.Sort((x ,y) => x.Name.CompareTo(y.Name));
                    }
                    else //desc
                    {
                        logTypesList.Sort((x, y) => y.Name.CompareTo(x.Name));
                    }
                    break;
            }
            var displayLogTypes = logTypesList.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            result = from i in displayLogTypes select new[] { i.Id.ToString(), i.Name, string.Join("; ", i.Headers) };
            totalRecords = logTypes.Length;

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = result
            },
            JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult ShowLogRecord(int id)
        {
            var logTypes = GetLogTypes();
            var logType = logTypes.Where(t => t.Id == id).FirstOrDefault();
            if (logType != null)
            {
                logType.Config.aoColumns = new object[logType.Headers.Length + 1];
                logType.Config.aoColumns[0] = new DataTableColumn() { sName = "Id" , bSearchable = false };
                for (int i = 0; i < logType.Headers.Length; ++i)
                {
                    logType.Config.aoColumns[i + 1] = new DataTableColumn() { sName = logType.Headers[i].ToString(), bSearchable = true };
                }
                return View(logType);
            }
            Response.StatusCode = 404;
            return null;
        }

        [AllowAnonymous]
        public async Task<ActionResult> LogRecordAjaxSource(jQueryDataTableParamModel param)
        {
            int id = 0;
            var q = HttpUtility.ParseQueryString(Request.UrlReferrer.Query);
            var stringId = q["id"];
            int.TryParse(stringId, out id);
            var logRecordsDb = (await logRecordRepository.FindByAsync(lr => lr.LogTypeId == id)).ToList();
            if (logRecordsDb != null)
            {
                var result = new List<string[]>();
                int totalRecords = 0;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    logRecordsDb = logRecordsDb.FindAll(x => LogRecordMatch(x, param.sSearch));
                }
                for (int i = 0; i < logRecordsDb.Count; ++i)
                {
                    var lr = logRecordsDb[i];
                    var record = new List<string> { lr.Id.ToString() };
                    record.AddRange(lr.GetContent());
                    result.Add(record.ToArray());
                }
                totalRecords = logRecordsDb.Count;
                if (param.sSortDir_0 == "asc")
                {
                    if(param.iSortCol_0 != 0)
                        result.Sort((x, y) => x[param.iSortCol_0].CompareTo(y[param.iSortCol_0]));
                    else
                        result.Sort((x, y) => Convert.ToInt32(x[param.iSortCol_0]).CompareTo(Convert.ToInt32(y[param.iSortCol_0])));
                }
                else //desc
                {
                    if (param.iSortCol_0 != 0)
                        result.Sort((x, y) => y[param.iSortCol_0].CompareTo(x[param.iSortCol_0]));
                    else
                        result.Sort((x, y) => Convert.ToInt32(y[param.iSortCol_0]).CompareTo(Convert.ToInt32(x[param.iSortCol_0])));
                }
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = totalRecords,
                    iTotalDisplayRecords = totalRecords,
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            Response.StatusCode = 404;
            return null;
        }

        [AllowAnonymous]
        public async Task<ActionResult> AddLogRecord(LogRecordAddViewModel model)
        {
            var response = new BaseModel();
            var logTypes = GetLogTypes();
            var logType = LogTypeFind(logTypes, model.Headers);
            if (logType != null)
            {
                var logTypeFromDB = (await logTypeRepository.FindByAsync(t => t.Id == logType.Id)).FirstOrDefault();
                if (logTypeFromDB != null)
                {
                    for (int i = 0; i < model.Records.Count; ++i)
                    {
                        var logRecord = new LogRecord();
                        logRecord.SetContent(model.Records[i]);
                        logTypeFromDB.Records.Add(logRecord);
                    }
                    logTypeRepository.Save();
                    logRecordRepository.Save();
                    response.SetSuccess("Log saved");
                    return Json(response);
                }
            }
            else
                response.SetError("Can't find log type");
            return Json(response);
        }

        [AllowAnonymous]
        public async Task<ActionResult> RemoveLogRecord(LogRecordRemoveViewModel model)
        {
            var entity = (await logRecordRepository.FindByAsync(lr => lr.Id == model.Id)).FirstOrDefault();
            logRecordRepository.Delete(entity);
            logRecordRepository.Save();
            model.SetSuccess("usunieto");
            return Json(model);

        }

        private void CreateCookie(User user)
        {
            FormsAuthentication.SetAuthCookie(user.Login, false);
            Response.Cookies.Clear();
            DateTime expDate = DateTime.Now.AddHours(2);
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    2, user.Login, DateTime.Now, expDate, true, string.Empty);
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
            HttpCookie authenticationCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            Response.Cookies.Add(authenticationCookie);
        }

        private LogTypeViewModel[] GetLogTypes()
        {
            LogTypeViewModel[] logTypes = this.HttpContext.Cache["LogTypes"] as LogTypeViewModel[];
            if (logTypes == null)
            {
                var dbLogTypes = logTypeRepository.GetAll();
                var dbLogTypesList = dbLogTypes.ToList();
                logTypes = Mapper.Map<List<LogType>, List<LogTypeViewModel>>(dbLogTypesList).ToArray();
                this.HttpContext.Cache["LogTypes"] = logTypes;
            }
            return logTypes;
        }

        private void ClearLogTypeCache()
        {
            this.HttpContext.Cache.Remove("LogTypes");
        }

        private bool LogTypeExist(LogTypeViewModel[] logTypes, LogTypeViewModel model)
        {
            if (logTypes != null && model != null)
            {
                for (int i = 0; i < logTypes.Length; ++i)
                {
                    var logType = logTypes[i];
                    if (logType.Headers != null && model.Headers != null && logType.Headers.Length == model.Headers.Length)
                    {
                        int counter = 0;
                        for (int j = 0; j < logType.Headers.Length; ++j)
                        {
                            if (logType.Headers[j] != model.Headers[j])
                                break;
                            else
                                counter++;
                        }
                        if (counter == model.Headers.Length)
                            return true;
                    }
                }
            }
            return false;
        }

        private LogTypeViewModel LogTypeFind(LogTypeViewModel[] logTypes, string[] headers)
        {
            if (logTypes != null && headers != null)
            {
                for (int i = 0; i < logTypes.Length; ++i)
                {
                    var logType = logTypes[i];
                    if (logType.Headers != null && logType.Headers.Length == headers.Length)
                    {
                        int counter = 0;
                        for (int j = 0; j < logType.Headers.Length; ++j)
                        {
                            if (logType.Headers[j] != headers[j])
                                break;
                            else
                                counter++;
                        }
                        if (counter == headers.Length)
                            return logType;
                    }
                }
            }
            return null;
        }

        private bool LogTypeMach(LogTypeViewModel model, string searchText)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                if (RemoveDiacritics(model.Name.ToLower()).Contains(RemoveDiacritics(searchText.ToLower())))
                    return true;
                else if (model.Headers != null)
                {
                    for (int i = 0; i < model.Headers.Length; ++i)
                    {
                        if (RemoveDiacritics(model.Headers[i].ToLower()).Contains(RemoveDiacritics(searchText.ToLower())))
                            return true;
                    }
                }
                return false;
            }
            return true;
        }

        private bool LogRecordMatch(LogRecord model, string searchText)
        {
            if (!string.IsNullOrEmpty(searchText))
            {
                string text = RemoveDiacritics(searchText.ToLower());
                var content = model.GetContent();
                for (int i = 0; i < content.Length; ++i)
                {
                    if (RemoveDiacritics(content[i].ToLower()).Contains(text))
                        return true;
                }
                return false;
            }
            return true;
        }

        private string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            var returnVal = stringBuilder.ToString().Normalize(NormalizationForm.FormC); //this doesn't remove ł and Ł so we must remove it manually
            returnVal = returnVal.Replace('ł', 'l');
            returnVal = returnVal.Replace('Ł', 'L');
            return returnVal;
        }
    }
}