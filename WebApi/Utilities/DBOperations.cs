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

        public static bool AddEmployee(Employee e)
        {
            try
            {
                using (PsiogEntities PE = new PsiogEntities())
                {

                    var max = PE.Employees.Max(m=>m.Id);
                    e.EmployeeId = "P" + max.ToString();
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

        public static bool AddReportingAuthorities(string empId,string authName)
        {
            try
            {
                using (PsiogEntities PE = new PsiogEntities())
                {
                    string authorityId = (from emp in PE.Employees
                                       where emp.Name==authName
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

        public static string VerifyUserAccount(string id, VerifyUser verifyEmp)
        {
            PsiogEntities PE = new PsiogEntities();

            var user = PE.Users.Where(u => u.EmployeeId == id).FirstOrDefault();
            user.Password = Hasher.HashString(verifyEmp.password);
            user.SecurityQuestion = verifyEmp.securityQuestion;
            user.Answer = verifyEmp.answer;
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

        public static string ResetPassword(string password,string id)
        {
            PsiogEntities PE = new PsiogEntities();

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

        public static string ApplyLeave(LeaveApplication leave)
        {
            PsiogEntities PE = new PsiogEntities();
            var leaveBalance = PE.EmployeeLeaveAvaliabilities.Where(emp => emp.EmployeeId == leave.EmployeeId && emp.LeaveTypeId == leave.LeaveId).FirstOrDefault();


            decimal availedDays = (leave.ToDate.Subtract(leave.FromDate)).Days;
            if (leave.FromSession == leave.ToSession)
                availedDays = availedDays + 0.5M;
            decimal balance = leaveBalance.AllocatedDays - leaveBalance.AvailedDays;
            if (availedDays <= balance)
            {
                leave.Status = "Applied";
                PE.LeaveApplications.Add(leave);
                leaveBalance.AvailedDays = availedDays;
                PE.SaveChanges();
                return "Leave Application Submitted! Waiting For Approval";
            }

            else
            {
                return "You do not have enough leave balance";
            }
        }
        /* public static EmployeeLeaveAvaliability CheckBalance(string employeeId)
             {
             PsiogEntities PE = new PsiogEntities();
             EmployeeLeaveAvaliability leaveBalance = PE.EmployeeLeaveAvaliabilities.Where(emp => emp.EmployeeId == employeeId).FirstOrDefault();
             return leaveBalance;
             /*
             decimal availedDays = (leave.ToDate.Subtract(leave.FromDate)).Days;
             if (leave.FromSession == leave.ToSession)
                 availedDays = availedDays + 0.5M;
             decimal balance = leaveBalance.AllocatedDays - leaveBalance.AvailedDays;
             return balance;

         } */


    }



}

