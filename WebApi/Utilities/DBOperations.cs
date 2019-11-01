using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using WebApi.Models;

namespace WebApi.Utilities
{
    public class DBOperations
    {

        public static bool AddEmployee(Employee e)
        {
            try
            {
                using (PsiogEntities PE = new PsiogEntities())
                {
                    PE.Employees.Add(e);

                    PE.SaveChanges();
                }
            }
            catch (Exception E)
            {
                ExceptionLog.Logger(E);
                return false;
            }
           
            return true;
        }
        public static bool AddUser(Employee e)
        {
            try
            {
                using (PsiogEntities PE = new PsiogEntities())
                {
                    User newUser = new User
                    {   EmployeeId = e.EmployeeId,
                        Password = Hasher.HashString(Guid.NewGuid().ToString()),
                        Role = "User",
                    };

                    PE.Users.Add(newUser);
                    PE.SaveChanges();
                    return true;
                }
            } catch(Exception E)
            {
                ExceptionLog.Logger(E);
                return false;
            }
           
        }

        public static string ValidateLogin(string username, string password, out User user)
        {
            try
            {
                using (PsiogEntities PE = new PsiogEntities())
                {
                    user = PE.Users.Where(u => u.EmployeeId == username).FirstOrDefault();
                    if (user != null)
                    {
                        if (string.Compare(Hasher.HashString(password), user.Password) == 0)
                        {
                            return "Login Successful!";
                        }
                        else
                        {
                            user = null;
                            return "Incorrect Password";
                        }
                    }
                    else
                    {
                        user = null;
                        return "Invalid Credentials";
                    }
                }
            }
            catch (Exception E)
            {
                ExceptionLog.Logger(E);
                user = null;
                return "Unable to connect to server!";
            }

         

        }

       
    }
}

