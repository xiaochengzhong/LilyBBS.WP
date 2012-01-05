﻿using System;
using System.Text;
using System.Text.RegularExpressions;

namespace LilyBBS.API
{
	/*
	public class LoginCompletedEventArgs : BaseEventArgs
	{
	}

	public delegate void LoginCompletedHandler(object sender, LoginCompletedEventArgs e);
	*/
	public class LoginRequest : BaseRequest
	{
		public LoginRequest(Connection connection, BaseHandler callback)
			: base(connection, callback)
		{
		}

		public void Login(string username, string password)
		{
			Random rand = new Random();
			connection.BaseUrl = string.Format("{0}vd{1}/", Connection.BbsUrl, new Random().Next(10000, 100000));
			ParameterList qry = new ParameterList();
			qry.Add("type", "2");
			ParameterList data = new ParameterList();
			data.Add("id", username);
			data.Add("pw", password);
			DoAction(LoginCompleted, "bbslogin", qry, data);
		}

		private void LoginCompleted(object sender, BaseEventArgs e)
		{
			Regex re = new Regex(@"setCookie\('(.*)'\)");
			string s = re.Match(e.Result as string).Groups[1].ToString();
			if (s == "")
			{
				// TODO
				throw new LilyError();
			}
			StringBuilder sb = new StringBuilder();
			string[] ss = s.Split(new char[] { '+' });
			sb.AppendFormat("_U_KEY={0}; ", int.Parse(ss[1]) - 2);
			ss = ss[0].Split(new char[] { 'N' });
			sb.AppendFormat("_U_UID={0}; ", ss[1]);
			sb.AppendFormat("_U_NUM={0}", int.Parse(ss[0]) + 2);
			String cookie = sb.ToString();
			connection.Cookie = cookie;
			connection.IsLoggedIn = true;
			if (callback != null)
			{
				callback(this, new BaseEventArgs(cookie, e.Error));
			}
		}
	}
}
