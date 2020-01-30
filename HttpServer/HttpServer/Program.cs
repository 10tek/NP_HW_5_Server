using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace HttpServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add("http://localhost/");
                listener.Start();
                Console.WriteLine("Сервер запущен");
                while (true)
                {
                    var context = await listener.GetContextAsync();
                    Console.WriteLine("Входящее соединение");

                    var response = context.Response;

                    using (var body = context.Request.InputStream)
                    using (var reader = new StreamReader(body, context.Request.ContentEncoding))
                    {
                        var json = await reader.ReadToEndAsync();
                        var user = JsonConvert.DeserializeObject<User>(json);
                        switch (context.Request.Url.AbsolutePath)
                        {
                            case "/user/signup":
                                if (context.Request.HttpMethod == "POST")
                                {
                                    response.StatusCode = await SignUp(user);
                                    response.ContentLength64 = 0;
                                }
                                else
                                {
                                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                                }
                                break;
                            case "/user/auth":
                                if (context.Request.HttpMethod == "POST")
                                {
                                    response.StatusCode = await SignIn(user);
                                    response.ContentLength64 = 0;
                                }
                                else
                                {
                                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                                }
                                break;
                            default:
                                response.StatusCode = (int)HttpStatusCode.BadRequest;
                                break;
                        }
                        response.OutputStream.Close();
                    }
                }
            }
        }

        public static async Task<int> SignUp(User user)
        {
            using (var context = new HwContext())
            {
                var dbUser = await context.Users.SingleOrDefaultAsync(x => x.Login == user.Login);
                if (dbUser != null)
                    return (int)HttpStatusCode.Forbidden;

                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
                return (int)HttpStatusCode.OK;
            }
        }

        public static async Task<int> SignIn(User user)
        {
            using (var context = new HwContext())
            {
                var dbUser = await context.Users.SingleOrDefaultAsync(x => x.Login == user.Login && x.Password == user.Password);
                if (dbUser is null)
                    return (int)HttpStatusCode.NotFound;

                return (int)HttpStatusCode.OK;
            }
        }
    }
}
