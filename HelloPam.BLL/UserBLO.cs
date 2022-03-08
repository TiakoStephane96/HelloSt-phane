using HelloPam.DAL;
using HelloStéphane.BO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HelloPam.BLL
{
    public class UserBLO
    {
        private readonly UserDAO userDAO;

        public UserBLO()
        {
            userDAO = new UserDAO();
            userDAO.Add();
        }

        public void CreateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("User");

            user.CreatedAt = DateTime.Now;

            if (user.Status == null)
                throw new ArgumentNullException("User status cannot be null !");
            if (user.Profile == null)
                throw new ArgumentNullException("User profile cannot be null !");

            userDAO.Add(user);
        }

        public void EditUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException("User");

            user.CreatedAt = DateTime.Now;

            if (user.Status == null)
                throw new ArgumentNullException("User status cannot be null !");
            if (user.Profile == null)
                throw new ArgumentNullException("User profile cannot be null !");

            userDAO.Set(user);
        }

        public void DeleteUser(int id)
        {
            userDAO.Delete(id);
        }

        public User GetUser(int id)
        {
           return userDAO.Get(id);
        }

        public User GetUser(string username, string password)
        {
            return userDAO.Login(username,password);
        }

        /// <summary>
        /// Authenticate a user by username and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>BO.User</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public User AuthenticateUser(string username, string password)
        {
            var users =  userDAO.Login(username, password);

            if (users == null)
                throw new KeyNotFoundException("Username or password is incorrect !");
            if (users.Status == false)
                throw new UnauthorizedAccessException("Your account is disabled !");
            return users;
        }

        public IEnumerable<User> FindUser(User user = null)
        {
            return userDAO.Find(user).OrderByDescending(x => x.CreatedAt).ToList();
        }
    }
}
