using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

public static class DBManager
{
    //当前SQL连接
    public static MySqlConnection mysql;
    //当前登录账户
    public static UserData currentUser;
    //本账户所有收藏的问题数据集合
    public static List<ColData> colDatas;

    //连接mysql数据库
    public static bool Connect(string db, string ip, int port, string user, string pw)
    {
        //创建MySqlConnection对象
        mysql = new MySqlConnection();
        //连接参数
        string s = string.Format("Database={0};Data Source={1}; " +
            "port={2};User Id={3}; Password={4}", db, ip, port, user, pw);
        mysql.ConnectionString = s;
        //连接
        try
        {
            mysql.Open();
            Debug.Log("数据库连接成功 ");

            return true;
        }
        catch (Exception e)
        {
            Debug.Log
                ("[DBManager.Connect()]数据库连接失败，异常为： " + e.Message);

            return false;
        }
    }

    //测试并重连
    private static void CheckAndReconnect()
    {
        try
        {
            if (mysql.Ping())
            {
                return;
            }
            mysql.Close();
            mysql.Open();
            Debug.Log("数据库重连成功！");
        }
        catch (Exception e)
        {
            Debug.Log
               ("[DBManager.CheckAndReconnect()]数据库重连异常为： " + e.Message);
        }
    }

    //判定安全字符串
    private static bool IsSafeString(string str)
    {
        return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|\*|!|\']");
    }

    //是否存在该用户（登录）
    public static bool IsAccountExist(string email)
    {
        CheckAndReconnect();
        //防sql注入
        if (!DBManager.IsSafeString(email))
        {
            PanelManager.Open<TipPanel>("请输入合法的邮箱账号！");

            return false;
        }
        //sql语句
        string s = string.Format("select * from t_user where email='{0}';", email);
        //查询
        try
        {
            MySqlCommand cmd = new MySqlCommand(s, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            bool hasRows = dataReader.HasRows;
            dataReader.Close();

            return !hasRows;
        }
        catch (Exception e)
        {
            Debug.Log
                ("[DBManager.IsAccountExist()]数据库读取异常为： " + e.Message);

            return false;
        }
    }

    //注册
    public static bool Regist(string email, string pw, string nn)
    {
        //CheckAndReconnect();
        //防sql注入
        if (!IsSafeString(email))
        {
            PanelManager.Open<TipPanel>("请输入合法的邮箱账号！");

            return false;
        }
        if (!IsSafeString(pw))
        {
            PanelManager.Open<TipPanel>("请输入合法的密码！");

            return false;
        }
        if (!IsSafeString(nn))
        {
            PanelManager.Open<TipPanel>("请输入合法的昵称！");

            return false;
        }
        //能否注册
        if (!IsAccountExist(email))
        {
            PanelManager.Open<TipPanel>("该邮箱注册的账户已存在！");

            return false;
        }
        //写入数据库t_User表
        string sql =
            $"insert into t_user set email ='{email}' ,password ='{pw}',nickname='{nn}';";
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception e)
        {
            PanelManager.Open<TipPanel>
                ("[DBManager.Regist()]用户注册失败，数据库异常为： " + e.Message);

            return false;
        }
    }

    //校验用户名密码
    public static bool CheckPassword(string email, string pw)
    {
        CheckAndReconnect();
        //防sql注入
        if (!IsSafeString(email))
        {
            PanelManager.Open<TipPanel>("请输入合法的邮箱账号！");

            return false;
        }
        if (!IsSafeString(pw))
        {
            PanelManager.Open<TipPanel>("请输入合法的密码！");

            return false;
        }

        //查询
        string sql = $"select * from t_user where email='{0}' and password='{1}';";

        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            bool hasRows = dataReader.HasRows;
            dataReader.Close();
            return !hasRows;
        }
        catch (Exception e)
        {
            Debug.Log
                ("[DBManager.CheckPassword()]数据库连接失败，异常为： " + e.Message);
            return false;
        }
    }

    //发送问题
    public static bool SendQue(string question, string content)
    {
        //写入数据库t_question表
        string sql =
            $"INSERT INTO `qasysdb`.`t_question` (`title`, `content`,`user_id`) VALUES('{question}', '{content}','{currentUser.id}');";
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();

            return true;
        }
        catch (Exception e)
        {
            PanelManager.Open<TipPanel>
                ("[DBManager.ReleaseQue()]用户发表问题失败，异常为： " + e.Message);

            return false;
        }
    }

    //收藏问题
    public static bool SendCol(string question_id)
    {
        //写入数据库t_collect表
        string sql =
            $"INSERT INTO `qasysdb`.`t_collect` (`user_id`, `question_id`) VALUES ('{currentUser.id}', '{question_id}');";
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();

            return true;
        }
        catch (Exception e)
        {
            PanelManager.Open<TipPanel>
                ("[DBManager.SendCol()]用户收藏问题失败，异常为： " + e.Message);

            return false;
        }
    }

    //发送回复
    public static ReplyData SendReply(string content, string question_id)
    {
        //写入数据库t_reply表
        string sql =
            $"insert into t_reply (content,question_id,user_id) values ('{content}', '{question_id}','{currentUser.id}') ;";

        string sql1 =
            $"select * from t_reply where content = '{content}' and question_id= '{question_id}' and user_id= '{currentUser.id}' ;";

        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();

            MySqlCommand cmd1 = new MySqlCommand(sql1, mysql);
            MySqlDataReader dataReader = cmd1.ExecuteReader();

            ReplyData rd = new ReplyData();

            if (!dataReader.HasRows)
            {
                dataReader.Close();
                return rd;
            }

            dataReader.Read();

            rd.id = dataReader.GetString("id");
            rd.content = dataReader.GetString("content");
            rd.question_id = dataReader.GetString("question_id");
            rd.user_id = dataReader.GetString("user_id");
            rd.goodcount = dataReader.GetString("goodcount");

            dataReader.Close();

            return rd;
        }
        catch (Exception e)
        {
            PanelManager.Open<TipPanel>
                ("[DBManager.ReleaseQue()]用户发表评论失败，异常为： " + e.Message);

            return null;
        }
    }

    //点赞回复
    public static bool SendGoodReply(string reply_id)
    {
        //写入数据库t_question表
        string sql =
            $"update t_reply set goodcount =goodcount+1 where id ={reply_id}";
        try
        {
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            cmd.ExecuteNonQuery();

            return true;
        }
        catch (Exception e)
        {
            PanelManager.Open<TipPanel>
                ("[DBManager.SendCol()]用户点赞失败，异常为： " + e.Message);

            return false;
        }
    }

    //取得用户数据
    public static void GetUserData(string email)
    {
        CheckAndReconnect();

        //防sql注入
        if (!IsSafeString(email))
        {
            PanelManager.Open<TipPanel>("您输入的邮箱不合法，请重新输入！");
        }

        string sql = $"select * from t_user where email ='{email}';";

        MySqlCommand cmd;
        MySqlDataReader dataReader;
        try
        {
            //查询
            cmd = new MySqlCommand(sql, mysql);
            dataReader = cmd.ExecuteReader();

            if (!dataReader.HasRows)
            {
                dataReader.Close();
            }
            //读取
            dataReader.Read();
            currentUser = new UserData();
            currentUser.id = dataReader.GetString("id");
            currentUser.email = email;
            currentUser.nickName = dataReader.GetString("nickname");
            dataReader.Close();
        }
        catch (Exception e)
        {
            Debug.Log("[DBManager.GetUserData()]获取用户数据失败，异常为" + e.Message);
        }
    }

    //取得所有问题
    public static List<QueData> GetAllQues()
    {
        CheckAndReconnect();

        List<QueData> QueDatas = new List<QueData>();

        string sql = $"select * from t_question;";
        try
        {
            //查询
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {
                //读取
                if (!dataReader.HasRows)
                {
                    dataReader.Close();
                    return QueDatas;
                }

                QueData qd = new QueData();

                qd.id = dataReader.GetString("id");
                qd.title = dataReader.GetString("title");
                qd.content = dataReader.GetString("content");
                qd.user_id = dataReader.GetString("user_id");
                qd.collectcount = dataReader.GetString("collectcount");
                qd.publishtime = dataReader.GetString("publishtime");

                QueDatas.Add(qd);
            }
            dataReader.Close();

            return QueDatas;
        }
        catch (Exception e)
        {
            Debug.Log
                ("[DBManager.GetAllQues()]获取所有问题失败，异常为" + e.Message);

            return QueDatas;
        }
    }

    //通过关键字检索所有问题
    public static List<QueData> GetAllKeyQues(string keyStr)
    {
        CheckAndReconnect();

        List<QueData> QueDatas = new List<QueData>();

        string sql = $"select * from t_question where title like'%{keyStr}%' ;";
        try
        {
            //查询
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {
                //读取
                if (!dataReader.HasRows)
                {
                    dataReader.Close();
                }

                Debug.Log(dataReader.GetString("id"));

                QueData qd = new QueData();

                qd.id = dataReader.GetString("id");
                qd.title = dataReader.GetString("title");
                qd.content = dataReader.GetString("content");
                qd.user_id = dataReader.GetString("user_id");
                qd.collectcount = dataReader.GetString("collectcount");
                qd.publishtime = dataReader.GetString("publishtime");

                QueDatas.Add(qd);
            }
            dataReader.Close();

            return QueDatas;
        }
        catch (Exception e)
        {
            Debug.Log
                ("[DBManager.GetUserData()]获取所有所带关键词的问题失败，异常为" + e.Message);

            return null;
        }
    }

    //取得该问题的所有回复
    public static List<ReplyData> GetAllRepliesByQueID(string queID)
    {
        CheckAndReconnect();

        List<ReplyData> replyDatas = new List<ReplyData>();

        string sql = $"select * from t_reply where question_id='{queID}';";
        try
        {
            //查询
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {
                if (!dataReader.HasRows)
                {
                    dataReader.Close();
                }

                ReplyData rd = new ReplyData();

                rd.id = dataReader.GetString("id");
                rd.content = dataReader.GetString("content");
                rd.question_id = dataReader.GetString("question_id");
                rd.user_id = dataReader.GetString("user_id");
                rd.goodcount = dataReader.GetString("goodcount");

                replyDatas.Add(rd);
            }
            dataReader.Close();

            return replyDatas;
        }
        catch (Exception e)
        {
            Debug.Log
                ("[DBManager.GetUserData()]获取所有回复数据失败，异常为" + e.Message);

            return null;
        }
    }

    //取得本账户所有的收藏数据
    public static List<ColData> GetAllCols()
    {
        CheckAndReconnect();

        colDatas = new List<ColData>();

        string sql = $"select * from t_collect where user_id='{currentUser.id}';";
        try
        {
            //查询
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {
                //判断游标位置
                if (!dataReader.HasRows)
                {
                    dataReader.Close();
                }

                ColData cd = new ColData();

                cd.id = dataReader.GetString("id");
                cd.question_id = dataReader.GetString("question_id");
                cd.user_id = dataReader.GetString("user_id");

                colDatas.Add(cd);
            }
            dataReader.Close();

            return colDatas;
        }
        catch (Exception e)
        {
            PanelManager.Open<TipPanel>
                ("[DBManager.GetAllCols()]获取当前用户所有的收藏问题失败，异常为" + e.Message);

            return null;
        }
    }

    //根据取得的本账户所有收藏问题集合的问题ID来获取问题的数据
    public static List<QueData> GetAllColQues()
    {
        //CheckAndReconnect();

        List<QueData> myColQuesDatas = new List<QueData>();

        StringBuilder allQueID = new StringBuilder();

        try
        {
            for (int i = 0; i < colDatas.Count; i++)
            {
                allQueID.Append(colDatas[i].question_id).Append(",");
            }

            if (allQueID.Length <= 0) return myColQuesDatas;

            allQueID.Remove(allQueID.Length - 1, 1);

            string sql = $"select * from t_question where id in (" + allQueID + ");";

            //查询
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();

            while (dataReader.Read())
            {
                //读取
                if (!dataReader.HasRows)
                {
                    dataReader.Close();
                }

                QueData qd = new QueData();

                qd.id = dataReader.GetString("id");
                qd.title = dataReader.GetString("title");
                qd.content = dataReader.GetString("content");
                qd.user_id = dataReader.GetString("user_id");
                qd.collectcount = dataReader.GetString("collectcount");
                qd.publishtime = dataReader.GetString("publishtime");

                myColQuesDatas.Add(qd);
            }

            dataReader.Close();

            return myColQuesDatas;
        }
        catch (Exception e)
        {
            PanelManager.Open<TipPanel>
                ("[DBManager.GetAllColQues()]获取本账户所有的收藏问题数据失败，异常为" + e.Message);

            return null;
        }
    }

    //取得所有本账号发布的问题信息
    public static List<QueData> GetAllMyQues()
    {
        CheckAndReconnect();

        MySqlDataReader dataReader;
        MySqlCommand cmd;

        List<QueData> myQue = new List<QueData>();

        try
        {
            string sql = $"select * from t_question where user_id= {currentUser.id};";

            //查询
            cmd = new MySqlCommand(sql, mysql);
            dataReader = cmd.ExecuteReader();

            while (dataReader.HasRows)
            {
                //读取
                dataReader.Read();

                QueData qd = new QueData
                {
                    id = dataReader.GetString("id"),
                    title = dataReader.GetString("title"),
                    content = dataReader.GetString("content"),
                    user_id = dataReader.GetString("user_id"),
                    collectcount = dataReader.GetString("collectcount"),
                    publishtime = dataReader.GetString("publishtime")
                };

                myQue.Add(qd);

                dataReader.Close();
            }

            return myQue;
        }
        catch (Exception e)
        {
            Debug.Log
                ("[DBManager.GetUserData()]获取本账户所有发布的问题数据失败，异常为" + e.Message);

            return null;
        }
    }

    //取得该问题ID对应的问题描述
    public static QueData GetQueDataByQueID(string question_id)
    {
        CheckAndReconnect();

        string sql = $"select * from t_question where id='{question_id}';";
        try
        {
            //查询
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();

            if (!dataReader.HasRows)
            {
                dataReader.Close();
            }
            dataReader.Read();

            QueData qd = new QueData();

            qd.id = dataReader.GetString("id");
            qd.title = dataReader.GetString("title");
            qd.content = dataReader.GetString("content");
            qd.user_id = dataReader.GetString("user_id");
            qd.collectcount = dataReader.GetString("collectcount");
            qd.publishtime = dataReader.GetString("publishtime");

            dataReader.Close();

            return qd;
        }
        catch (Exception e)
        {
            Debug.Log
                ("[DBManager.GetQueDataByQueID()]获取当前问题描述失败，异常为" + e.Message);

            return null;
        }
    }

    //根据用户ID和问题ID查询t_reply表中的回复数据
    public static ReplyData GetReplyByUserAndQueID(string user_id, string question_id)
    {
        CheckAndReconnect();

        string sql = $"select * from t_reply where user_id='{user_id}' and question_id='{question_id}';";
        try
        {
            //查询
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();

            if (!dataReader.HasRows)
            {
                dataReader.Close();
            }

            dataReader.Read();
            ReplyData rd = new ReplyData();

            rd.id = dataReader.GetString("id");
            rd.content = dataReader.GetString("content");
            rd.question_id = dataReader.GetString("question_id");
            rd.user_id = dataReader.GetString("user_id");
            rd.goodcount = dataReader.GetString("goodcount");

            dataReader.Close();

            return rd;
        }
        catch (Exception e)
        {
            Debug.Log
                ("[DBManager.GetReplyByUserAndQueID()]获取当前回复的信息失败，异常为" + e.Message);

            return null;
        }
    }

    //根据回复id查找回复的数据
    public static ReplyData GetReplyByID(string reply_id)
    {
        CheckAndReconnect();

        string sql = $"select * from t_reply where id='{reply_id}';";
        try
        {
            //查询
            MySqlCommand cmd = new MySqlCommand(sql, mysql);
            MySqlDataReader dataReader = cmd.ExecuteReader();

            if (!dataReader.HasRows)
            {
                dataReader.Close();
            }

            dataReader.Read();
            ReplyData rd = new ReplyData();

            rd.id = dataReader.GetString("id");
            rd.content = dataReader.GetString("content");
            rd.question_id = dataReader.GetString("question_id");
            rd.user_id = dataReader.GetString("user_id");
            rd.goodcount = dataReader.GetString("goodcount");

            dataReader.Close();

            return rd;
        }
        catch (Exception e)
        {
            PanelManager.Open<TipPanel>
                ("[DBManager.GetReplyByID()]获取当前回复的信息失败，异常为" + e.Message);

            return null;
        }
    }
}