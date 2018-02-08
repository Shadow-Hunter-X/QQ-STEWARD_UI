/* ----------------------------------------------------------------------------------------
 *  文件名 :CustomerHttpModules.cs
 * 
 *  作用   : 完成URL重定向到SSO服务器，传送信息加密
 * 
 *  时间   : 2016/12/27
 *
 *  说明   : 主要三个接口或类 IHttpModule  HttpApplication HttpHandler
 *
 *  针对HttpApplication的操作较多: 查看MSDN
 *  https://msdn.microsoft.com/zh-cn/library/system.web.httpapplication(v=vs.110).aspx 

 * ----------------------------------------------------------------------------------------
 *
 * IhttpMododule实现类中的注意点:
 *  FormsAuthenticationModule  AuthenticateRequest  
 *  确定用户是否通过了窗体身份验证。如果没有，用户将被自动重定向到指定的登录页面。
 *
 *  FileAuthorizationMoudle    AuthorizeRequest 
 *  使用 Windows 身份验证时，此 HTTP 模块将检查以确保Windows帐户对被请求                                                                             的资源具有足够的权限。
 *
 *  UrlAuthorizationModule    AuthorizeRequest              
 *  检查以确保请求者可以访问指定的 URL。通过 Web.config 文件中的<authorization>和<location>元素来指定 URL 授权。 
 *  
 * ----------------------------------------------------------------------------------------*/
using System;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;
using CWRedirectDll;
using System.Security.Principal;
using RsaUtil;
using System.Web.Security;

namespace CustomerHttpModules
{
    public class MyHttpModules : IHttpModule
    {

        // SSO服务器需要信息 
        private string AppId;
        private string AuthType;
        private string AccountType;
        private string AuthUrl;

        private string homepage;
        private string SignOutPage;
        private string publickey;
        private string privatekey;

        // RSA加密中有长度限制
        private const int DWKEYSIZE = 1024;

        public MyHttpModules()
        {

        }

        /// <summary>
        /// init 初始化模块，并使其为处理请求做好准备,并读取Web.config中的配置信息
        /// </summary>
        /// <param name="context"></param>
        void System.Web.IHttpModule.Init(HttpApplication context)
        {

            // 在 ASP.NET 响应请求时作为 HTTP 执行管线链中的第一个事件发生。
            context.BeginRequest += new EventHandler(context_BeginRequest);

            // 当 ASP.NET 获取与当前请求关联的当前状态（如会话状态）时发生。
            //context.AcquireRequestState += new EventHandler(context_AcquireRequestState);

            // 读取配置信息
            ConfigSection config = (ConfigSection)ConfigurationManager.GetSection("SSOClient");

            AppId = config.AppId.ToString();

            AuthType = config.SSOParam.AuthType.ToString();

            AccountType = config.SSOParam.AccoutType.ToString();

            AuthUrl = config.SSOParam.AuthUrl.ToString();

            homepage = config.UrlParam.homepage.ToString();

            SignOutPage = config.UrlParam.SignOutPage.ToString();

            publickey = config.PathParam.publickey.ToString();

            privatekey = config.PathParam.privatekey.ToString();

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        public void context_BeginRequest(object obj, System.EventArgs e)
        {
            HttpApplication application = (HttpApplication)obj;
            HttpContext contexta = application.Context;
            string username = null;
            string requestUrl = contexta.Request.QueryString["Account"];
            string SignInfor  = contexta.Request.QueryString["Sign"];
            if (requestUrl != null)
            {
                UTF8Encoding a = new UTF8Encoding();
                string res = HttpUtility.UrlDecode(requestUrl, a);
                byte[] data = Convert.FromBase64String(res);

                string signstr = HttpUtility.UrlDecode(SignInfor);
                byte[] signData = Convert.FromBase64String(signstr);
                if (RSAUtil.VerifyHash(RSAUtil.GetPublicKeyFromXml(publickey), data, signData))
                {
                    try
                    {
                        RSAParameters param = RSAUtil.GetPrivateKeyFromXml(privatekey);
                        byte[] Adestr = RSAUtil.RSADecrypt(data, param, false);
                        username = Encoding.UTF8.GetString(Adestr);
                    }
                    // 解密失败
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message.ToString());
                    }
                    if (username != null)
                    {
                        // 生成对应的用户对象实例
                        GenericPrincipal user = handle_validUserName(username, application);

                        // 检查对应用户的Cookie是否生成，如无则生成,有则不作任何操作
                        if (gen_usercookie(application, user))
                        {
                            application.Response.Redirect(homepage);
                        }
                        else
                        {
                            throw new Exception("生成用户Cookie不成功");
                        }
                    }
                    else if (username == null)
                    {
                        // 向SSO服务器重定向信息
                        if (!handle_invalidUserName(application))
                        {
                            throw new Exception("向服务器发送重定向信息失败");
                        }
                    }
                }
            }
            else if (requestUrl == null)
            {
                if (!handle_invalidUserName(application))
                {
                    throw new Exception("向服务器发送重定向信息失败");
                }
            }
        }

        /// <summary>
        /// 处理有效用户名，生成对应的用户对象
        /// </summary>
        /// <param name="username"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private GenericPrincipal handle_validUserName(string username, HttpApplication app)
        {

            //HttpContext context = app.Context;

            // 用户角色 1管理员  2普通用户
            string[] roles = { "1", "2" };
            try
            {
                // 构造用户
                GenericIdentity identity = new GenericIdentity(username);

                // 分配对象的权限
                GenericPrincipal principal = new GenericPrincipal(identity, roles);

                // 将用绑定到当前HTTP请求线程,接下来的cookie操作在context_AcquireRequestState中进行操作
                HttpContext.Current.User = principal;

                return principal;
            }
            catch (Exception ex)
            {
                // 写日志操作
                return null;
            }

        }

        /// <summary>
        /// 没有用户名信息，需要重定向到SSO服务器
        /// </summary>
        /// <returns></returns>
        private bool handle_invalidUserName(HttpApplication app)
        {
            if (app.Request.Cookies[FormsAuthentication.FormsCookieName] == null)
            {

                string url = app.Request.Url.ToString();
                // 进行重定向到SSO登录界面。
                string strurl = "ReturnUrl=" + url + "&AppId=" + AppId + "&AuthType=" + AuthType + "&AccountType=" + AccountType;

                // 判断加密原文是否超出长度
                if (RSAUtil.CheckSourceValidate(strurl))
                {
                    byte[] dataToEncrypt = Encoding.UTF8.GetBytes(strurl);
                    byte[] EncryptedUrl;
                    try
                    {
                        // 加密
                        EncryptedUrl = RSAUtil.RSAEncrypt(dataToEncrypt, RSAUtil.GetPublicKeyFromXml(publickey), false);
                    
                        // 加密数据签名
                        byte[] SignByte = RSAUtil.HashAndSign(EncryptedUrl,RSAUtil.GetPrivateKeyFromXml(privatekey));

                        // 转Base64,便于HTTP传送
                        string base64url = Convert.ToBase64String(EncryptedUrl);
                        string base64Sign = Convert.ToBase64String(SignByte);
                        base64url = transformSomeSign(base64url);
                        base64Sign = transformSomeSign(base64Sign);

                        // 重定向到SSO服务器
                        app.Response.Redirect(AuthUrl + "&request=" + base64url + "&sign=" + base64Sign);
                    }
                    catch (Exception e)
                    {
                        // 写日志操作
                        throw new CryptographicException(e.Message.ToString());
                    }
                    return true;
                }
                else
                {
                    // 添加些日志操作
                    return false;
                }
            }
            else
            {
                //if (get_currentNameFromCookie(app))
                //    return true;
                //else
                    return true;
            }
        }


        /// <summary>
        /// 生成用户Cookie
        /// </summary>
        /// <param name="app">当前请求HttpApplication</param>
        /// <param name="user">需生成Cookie的用户名</param>
        /// <returns>成功生成true  否则false</returns>
        private bool gen_usercookie(HttpApplication app, GenericPrincipal user)
        {
            if (app.Request.Cookies[FormsAuthentication.FormsCookieName] == null)
            {
                FormsAuthenticationTicket ticket = CreateAuthenticationTicket(user.Identity.Name, "32977dcd-4096-4bfa-8f00-02154fcc7bf8,5d2d95aa-f45c-4ba2-a014-9547c6b67f84", true, "SSOCookiePath");
                if (ticket != null)
                {
                    string encrypetedTicket = FormsAuthentication.Encrypt(ticket);

                    try
                    {
                        if (!FormsAuthentication.CookiesSupported)
                        {
                            // 为提供的用户名创建一个身份验证票证，并将其添加到响应的 Cookie 集合或 URL。
                            FormsAuthentication.SetAuthCookie(encrypetedTicket, true);
                        }
                        else
                        {
                            // 更好的方法是将Cookie名字设置在配置文件中。
                            // 设置Cookie名为CWSSOClient对应的值为加密的ticket
                            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypetedTicket);

                            // IsPersistent 获取一个值，该值指示包含 Forms 身份验证票证信息的 Cookie 是否为持久性的。
                            /*
                            if (ticket.IsPersistent)
                            {
                                //Expires获取或设置此 Cookie 的过期日期和时间
                                //Expiration获取 Forms 身份验证票证过期时的本地日期和时间。
                                authCookie.Expires = ticket.Expiration;
                            }
                            */
                            // Response.Cookies 获取响应 Cookie 集合。
                            // 将当前的Cookie添加到Cookie集合，并Response返回
                            HttpContext.Current.Response.Cookies.Add(authCookie);

                        }
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                    return false;
            }
            else
                return true;
        }


        /// <summary>
        /// 获取当前用户的username,附带相关的ticket，构造对象模型绑定到当前HTTP请求线程上。
        /// </summary>
        /// <param name="app">当初线程处理的HTTP请求的对象</param>
        /// <returns></returns>
        private bool get_currentNameFromCookie(HttpApplication app)
        {
            if (HttpContext.Current.User == null)
            {
                try
                {

                    HttpCookie cookie = app.Request.Cookies[FormsAuthentication.FormsCookieName];

                    string ticketstr = cookie.Value;

                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(ticketstr);

                    string username = ticket.Name;

                    string[] roles = { "1", "2" };

                    FormsIdentity formuser = new FormsIdentity(ticket);

                    GenericIdentity identity = new GenericIdentity(username);

                    // 分配对象的权限
                    GenericPrincipal principal = new GenericPrincipal(formuser, roles);


                    // 将用绑定到当前HTTP请求线程,接下来的cookie操作在context_AcquireRequestState中进行操作
                    HttpContext.Current.User = principal;

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            else
                return true;

        }

        /// <summary>
        /// 用户生成tikcet
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="commaSeperatedRoles">用户角色</param>
        /// <param name="createPersistentCookie">是否创建持久Cookie</param>
        /// <param name="strCookiePath">Cookie在服务器端保存的位置</param>
        /// <returns></returns>
        private FormsAuthenticationTicket CreateAuthenticationTicket(string userName, string commaSeperatedRoles, bool createPersistentCookie, string strCookiePath)
        {
            string cookiePath = strCookiePath == null ? FormsAuthentication.FormsCookiePath : strCookiePath;

            int expirationMinutes = 30;

            //使用 cookie 名、版本、目录路径、发布日期、过期日期、持久性以及用户定义的数据初始化 FormsAuthenticationTicket 类的新实例
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
            1,
            userName,
            DateTime.Now,
            DateTime.Now.AddMinutes(expirationMinutes),
            createPersistentCookie,
            commaSeperatedRoles);

            return ticket;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string transformSomeSign(string text)
        {
            text = text.Replace("+", "%2B");
            text = text.Replace(" ", "+");
            text = text.Replace("/", "%2F");
            text = text.Replace("?", "%3F");
            text = text.Replace("%", "%25");
            text = text.Replace("#", "%23");
            text = text.Replace("&", "%26");
            text = text.Replace("=", "%3D");
            return text;
        }


        /// <summary>
        /// 当 ASP.NET 获取与当前请求关联的当前状态（如会话状态）时发生的事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /*
        void context_AcquireRequestState(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;

            // currentUser表示当前用户，可以从器属性中读取username属性
            //CustomPrincipal currentUser = application.Context.User as CustomPrincipal;

            // 1 读取cookie操作。
            
            if (application.Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                application.Response.Redirect(application.Request.Url.ToString(),false);
                //application.Response.Redirect("http://localhost:3912/");
            }
        }*/

        public void Dispose()
        {
            // TODO:   添加 MyHttpModules.Dispose 实现 
        }
    }

    /// <summary>
    /// Web端，读取Web.config配置文件中的配置信息
    /// </summary>
    public class ConfigSection : ConfigurationSection
    {
        public ConfigSection()
        {

        }

        [ConfigurationProperty("AppId", DefaultValue = "HR", IsRequired = true)]
        public string AppId
        {
            get { return (string)this["AppId"]; }
            set { this["AppId"] = value; }
        }

        [ConfigurationProperty("infor")]
        public Paraminfo SSOParam
        {
            get { return (Paraminfo)this["infor"]; }
            set { this["SSOParam"] = value; }
        }


        [ConfigurationProperty("url")]
        public UrlParam UrlParam
        {
            get { return (UrlParam)this["url"]; }
            set { this["UrlParam"] = value; }
        }

        [ConfigurationProperty("path")]
        public PathParam PathParam
        {
            get { return (PathParam)this["path"]; }
            set { this["PathParam"] = value; }
        }


    }

    /// <summary>
    /// 代表web.config节点下的 <SSOClient>节点下的inofr节点
    /// </summary>
    public class Paraminfo : ConfigurationElement
    {
        public Paraminfo() { }

        [ConfigurationProperty("AuthUrl", IsRequired = true)]
        public string AuthUrl
        {
            get { return (string)this["AuthUrl"]; }
        }

        [ConfigurationProperty("AuthType", IsRequired = true)]
        public string AuthType
        {
            get { return (string)this["AuthType"]; }
        }

        [ConfigurationProperty("AccountType", IsRequired = true)]
        public string AccoutType
        {
            get { return (string)this["AccountType"]; }
        }

    }

    public class UrlParam : ConfigurationElement
    {
        public UrlParam() { }

        [ConfigurationProperty("homepage", IsRequired = true)]
        public string homepage
        {
            get { return (string)this["homepage"]; }
        }

        [ConfigurationProperty("SignOutPage", IsRequired = true)]
        public string SignOutPage
        {
            get { return (string)this["SignOutPage"]; }
        }

    }

    public class PathParam : ConfigurationElement
    {
        public PathParam() { }

        [ConfigurationProperty("publickey", IsRequired = true)]
        public string publickey
        {
            get { return (string)this["publickey"]; }
        }

        [ConfigurationProperty("privatekey", IsRequired = true)]
        public string privatekey
        {
            get { return (string)this["privatekey"]; }
        }


    }


}
