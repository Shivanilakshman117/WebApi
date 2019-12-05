using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Utilities
{
    public class DBOperations
    {

        public static bool AddEmployee(Employee e, out string msg)
        {
            msg = "";
            
            try
            {
                using (PsiogEntities PE = new PsiogEntities())
                {
                    var check = PE.Employees.Where(c => c.Email == e.Email).FirstOrDefault();
                    if(check!=null)
                    {
                        msg = "Email ID already exists!";
                        return false;
                    }
                    var max = PE.Employees.Max(m=>m.Id) + 1;
                    e.EmployeeId = "P" + max.ToString();
                    PE.Employees.Add(e);

                    PE.SaveChanges();
                }
            }
            catch (Exception E)
            {
                ExceptionLog.Logger(E);
                msg = "Failed to add employee";
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
                        VerificationCode = Guid.NewGuid().ToString(),
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

        public static bool AddReportingAuthorities(string empId,string authName)
        {
            if (authName != null)
            {
                try
                {
                    using (PsiogEntities PE = new PsiogEntities())
                    {
                        string authorityId = (from emp in PE.Employees
                                              where emp.Name == authName
                                              select emp.EmployeeId).FirstOrDefault();
                        if (authorityId != null)
                        {
                            ReportingAuthority RA = new ReportingAuthority
                            {
                                EmployeeId = empId,
                                ManagerId = authorityId
                            };

                            PE.ReportingAuthorities.Add(RA);
                            PE.SaveChanges();
                            return true;

                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception E)
                {
                    ExceptionLog.Logger(E);

                    return false;
                }
            }
            return true;

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

        public static string VerifyUserAccount(string encodedId, VerifyUser verifyEmp)
        {
            using (PsiogEntities PE = new PsiogEntities())
            {
                var id = Hasher.DecodeId(encodedId);
                var user = PE.Users.Where(u => u.EmployeeId == id).FirstOrDefault();
                if (user.VerificationCode == null)
                {
                    return "Unauthorised!";
                }
                user.Password = Hasher.HashString(verifyEmp.password);
                user.SecurityQuestion = verifyEmp.securityQuestion;
                user.Answer = verifyEmp.answer;
                user.VerificationCode = null;
                try
                {

                    PE.SaveChanges();
                    return "Verified Successfully!";
                }
                catch (Exception E)
                {
                    ExceptionLog.Logger(E);
                    return "Unable to verify user";
                }
            }
        }

        public static string ResetPassword(string password,string id)
        {
            using (PsiogEntities PE = new PsiogEntities())
            {
                var user = PE.Users.Where(u => u.EmployeeId == id).FirstOrDefault();
                user.Password = Hasher.HashString(password);

                try
                {

                    PE.SaveChanges();
                    return "Updated password successfully";
                }
                catch (Exception E)
                {
                    ExceptionLog.Logger(E);
                    return "Unable to update password. Please try again";
                }
            }
        }

        public static string ApplyLeave(LeaveApplication leave, string id)
        {

            using (PsiogEntities PE = new PsiogEntities())
            {
                try
                {
                    var leaveBalance = PE.EmployeeLeaveAvaliabilities.Where(emp => emp.EmployeeId == id && emp.LeaveTypeId == leave.LeaveId).FirstOrDefault();


                    decimal availedDays = (leave.ToDate.Subtract(leave.FromDate)).Days;
                    if (leave.FromSession == leave.ToSession)
                        availedDays = availedDays + 0.5M;
                    decimal balance = leaveBalance.AllocatedDays - leaveBalance.AvailedDays;
                    if (availedDays <= balance)
                    {
                        try
                        {
                            var l = new LeaveApplication();
                            leave.Status = "Applied";
                            l.Status = leave.Status;
                            l.FromDate = leave.FromDate;
                            l.ToDate = leave.ToDate;
                            l.FromSession = leave.FromSession;
                            l.ToSession = leave.ToSession;
                            l.Reason = leave.Reason;
                            l.LeaveId = leave.LeaveId;
                            l.SendTo = leave.SendTo;
                            l.EmployeeId = id;


                            PE.LeaveApplications.Add(l);
                            leaveBalance.AvailedDays = availedDays;
                            PE.SaveChanges();
                            return "Leave Application Submitted! Waiting For Approval";
                        }
                        catch (Exception E)
                        {
                            ExceptionLog.Logger(E);
                            return "Unable to apply leave";
                        }
                    }

                    else
                    {
                        return "You do not have enough leave balance";
                    }
                }
                catch(Exception E)
                {
                    ExceptionLog.Logger(E);
                    return "Unable to apply leave. Please try again";

                }
            }
        }



    }



}

