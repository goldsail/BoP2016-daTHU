using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Web;
using System.Net.Sockets;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization.Formatters;

namespace ConsoleApplication
{
    [DataContract]
    class Out0
    {
        [DataMember]
        internal IDClass[] entities;
    }

    [DataContract]
    class IDClass
    {
        [DataMember]
        internal Int64 Id;
        [DataMember]
        internal Int64[] RId;
        [DataMember]
        internal AAClass[] AA;
        [DataMember]
        internal FClass[] F;
        [DataMember]
        internal JClass J;
        [DataMember]
        internal CClass C;
    }

    [DataContract]
    class AAClass
    {
        [DataMember]
        internal Int64 AuId;
        [DataMember]
        internal Int64 AfId;
    }

    [DataContract]
    class CClass
    {
        [DataMember]
        internal Int64 CId;
    }

    [DataContract]
    class FClass
    {
        [DataMember]
        internal Int64 FId;
    }

    [DataContract]
    class JClass
    {
        [DataMember]
        internal Int64 JId;
    }

    public class Obj
    {

        public String Type; // Paper, Author, Journal, Conference, Field, AuthorField.
        public Int64 ID;
        public void Print()
        {
            Console.WriteLine("class Obj.   Type: " + Type + ",   ID: " + ID.ToString());
        }

        public bool Equals(Obj o)
        {
            if (Type == o.Type && ID == o.ID)
                return true;
            else
                return false;
        }
        public Obj()
        {
            Type = "Paper";
            ID = 0;
        } // 默认构造函数
        public Obj(string str, Int64 id)
        {
            Type = str;
            ID = id;
        } // 构造函数，输入Type和ID
        public Obj(string str)
        {
            Type = "Paper";
            ID = 0;
            char[] c = str.ToCharArray();
            int n = str.Length;
            int p = 0; // stands for the position of the column ":"
            string t1 = "";
            string t2 = "";
            if (n > 1)
            {
                for (int i = 0; i < n; i++)
                {
                    if (c[i] == ':')
                    {
                        p = i;
                        break;
                    }
                }
                if (p > 1 && p < n - 1)
                {
                    for (int i = 1; i < p - 1; i++)
                    {
                        t1 = String.Concat(t1, c[i].ToString());
                    }
                    for (int i = p + 1; i < n; i++)
                    {
                        t2 = String.Concat(t2, c[i].ToString());
                    }
                    Int64 t = Int64.Parse(t2);
                    ID = t;
                    //Console.WriteLine(t1);
                    switch (t1)
                    {
                        case "C.Id":
                            Type = "Conference";
                            break;
                        case "J.Id":
                            Type = "Journal";
                            break;
                        case "F.FId":
                            Type = "Field";
                            break;
                        case "AA.AfId":
                            Type = "AuthorField";
                            break;
                        case "AuId":
                            Type = "Author";
                            break;
                        case "Id":
                            Type = "Paper";
                            break;
                        default:
                            break;
                    }
                }
            }
        } // 构造函数，输入json格式的字符串，例如："C.Id":2345
        public string ToStr() // parse to string in json format
        {
            string result = "";
            ;
            switch (Type)
            {
                case "Paper":
                    result += "\"Id\":";
                    break;
                case "Author":
                    result += "\"AuId\":";
                    break;
                case "Conference":
                    result += "\"C.Id\":";
                    break;
                case "Journal":
                    result += "\"J.Id\":";
                    break;
                case "Field":
                    result += "\"F.FId\":";
                    break;
                case "AuthorField":
                    result += "\"AA.AfId\":";
                    break;
            }
            result += ID.ToString();
            return result;
        }
    }

    class Program
    {

        static void response()
        {
            while (true)
            {
                using (HttpListener listener = new HttpListener())
                {
                    listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;//指定身份验证 Anonymous匿名访问

                    listener.Prefixes.Add("http://dathu.chinacloudapp.cn/");
                    listener.Start();
                    Console.WriteLine("WebServer Start Succeeded.......");

                    while (true)
                    {

                        //等待请求连接
                        //没有请求则GetContext处于阻塞状态

                        HttpListenerContext ctx = listener.GetContext();
                        //
                        string id1 = ctx.Request.QueryString["id1"].ToString();
                        string id2 = ctx.Request.QueryString["id2"].ToString();
                        Console.WriteLine(id1);
                        Console.WriteLine(id2);

                        Int64 ID1 = 0;
                        Int64 ID2 = 0;
                        try
                        {
                            ID1 = Int64.Parse(id1);
                            ID2 = Int64.Parse(id2);
                            Console.WriteLine(ID1);
                            Console.WriteLine(ID2);

                            ctx.Response.StatusCode = 200;//设置返回给客服端http状态代码
                        }
                        catch
                        {
                            Console.WriteLine("Bad Request Format");
                            ctx.Response.StatusCode = 500;
                            continue;
                        }

                        // 先判断id是什么类型：用Id=请求AuId属性，返回的结果出现AuId字段就是Id，否则是AuId
                        Console.WriteLine("Type configuration started");
                        string T1 = "Paper";
                        string T2 = "Paper";
                        string _temp1 = GetRequest("Id=" + id1, "AA.AuId", 100);
                        string _temp2 = GetRequest("Id=" + id2, "AA.AuId", 100);
                        char[] _tmp1 = _temp1.ToCharArray();
                        char[] _tmp2 = _temp2.ToCharArray();
                        int _count1 = _tmp1.Length;
                        int _count2 = _tmp2.Length;
                        int _flag1 = 0;
                        int _flag2 = 0;
                        //
                        if (_count1 > 1)
                        {
                            for (int i = 0; i < _count1 - 1; i++)
                            {
                                if (_tmp1[i] == 'A' && _tmp1[i + 1] == 'A')
                                {
                                    _flag1 = 1;
                                    break;
                                }
                            }
                        }
                        if (_flag1 == 1)
                            T1 = "Paper";
                        else
                            T1 = "Author";
                        //
                        if (_count2 > 1)
                        {
                            for (int i = 0; i < _count2 - 1; i++)
                            {
                                if (_tmp2[i] == 'A' && _tmp2[i + 1] == 'A')
                                {
                                    _flag2 = 1;
                                    break;
                                }
                            }
                        }
                        if (_flag2 == 1)
                            T2 = "Paper";
                        else
                            T2 = "Author";
                        //
                        Console.WriteLine("Type configuration finished");

                        Obj dept = new Obj(T1, ID1);
                        Obj dest = new Obj(T2, ID2);
                        dept.Print();
                        dest.Print();

                        ArrayList result = new ArrayList();
                        try
                        {
                            // result = SearchPath(dept, dest);
                            result = nSearch(dept, dest);
                        }
                        catch
                        {
                            Console.WriteLine("Internal Error");
                            ctx.Response.StatusCode = 500;
                            continue;
                        }
                        //

                        //使用Writer输出http响应代码
                        using (StreamWriter writer = new StreamWriter(ctx.Response.OutputStream))
                        {
                            writer.Write("[");
                            foreach (ArrayList a in result)
                            {
                                if (a.Count < 2 || a.Count > 4)
                                    continue;
                                if (result.IndexOf(a) > 0)
                                    writer.Write(",");
                                writer.Write("[");
                                switch (a.Count)
                                {
                                    case 2:
                                        Obj t0 = a[0] as Obj;
                                        Obj t1 = a[1] as Obj;
                                        writer.Write(t0.ID.ToString() + "," + t1.ID.ToString());
                                        break;
                                    case 3:
                                        Obj t2 = a[0] as Obj;
                                        Obj t3 = a[1] as Obj;
                                        Obj t4 = a[2] as Obj;
                                        writer.Write(t2.ID.ToString() + "," + t3.ID.ToString() + "," + t4.ID.ToString());
                                        break;
                                    case 4:
                                        Obj t5 = a[0] as Obj;
                                        Obj t6 = a[1] as Obj;
                                        Obj t7 = a[2] as Obj;
                                        Obj t8 = a[3] as Obj;
                                        writer.Write(t5.ID.ToString() + "," + t6.ID.ToString() + "," + t7.ID.ToString() + "," + t8.ID.ToString());
                                        break;
                                    default:
                                        continue;
                                }
                                writer.Write("]");
                            }

                            writer.Write("]");
                            writer.Close();

                            ctx.Response.Close();
                        }

                    }
                    listener.Stop();

                }
            }
        } // 监听端口、解析请求，调用SearchPath函数、返回响应

        static bool isLegalPath(string prev, string next)
        {
            switch (prev)
            {
                case "Paper":
                    switch (next)
                    {
                        case "Paper": return true;
                        case "Conference": return true;
                        case "Journal": return true;
                        case "Field": return true;
                        case "Author": return true;
                    }
                    break;
                case "Author":
                    switch (next)
                    {
                        case "Paper": return true;
                        case "AuthorField": return true;
                    }
                    break;
                case "Field":
                    if (next == "Paper")
                        return true;
                    break;
                case "Conference":
                    if (next == "Paper")
                        return true;
                    break;
                case "Journal":
                    if (next == "Paper")
                        return true;
                    break;
                case "AuthorField":
                    if (next == "Author")
                        return true;
                    break;
            }
            return false;
        }

        static void Main(string[] args)
        {
            
            while (true)
            {
                try
                {
                    response();
                }
                catch
                {
                    continue;
                }
            }
            

            Obj test = new Obj("AuthorField", 56590836);
            //ArrayList temp = Request(test);
            //Console.WriteLine(temp.Count);

            Obj test0 = new Obj("Paper", 2059036641);
            Out0 test00 = Request(test0);
            // string temp = GetRequest("Composite(AA.AuId=2134693834)", "Id%2cAA.AuId%2cAA.AfId%2cF.FId%2cRId%2cJ.JId%2cC.CId");
            // ArrayList tmp = ParseRequest(temp);
            //string a = "{\"age\":[42, 36],\"name\":\"John\"}";
            //Out0 p = new Out0();
            //p.age = "dsc";
            //p.name = "dfas";
            //MemoryStream stream1 = new MemoryStream(Encoding.Default.GetBytes(a));
            //stream1.Read(, 0, a.Length);
            //DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Person));
            //ser.ReadObject(stream1, p);
            //Out0 b = ser.ReadObject(stream1) as Out0;

            //Obj test1 = new Obj("Paper", 2114983066);
            //Obj test2 = new Obj("Paper", 2100188138);
            Obj test1 = new Obj("Paper", 189831743);
            Obj test2 = new Obj("Paper", 41008148);

            ArrayList res = SearchPath(test1, test2);
            Console.WriteLine(res.Count.ToString());

            //Out0 response0 = Request(test1);

            //response();
            // string _temp1 = GetRequest("Id=" + 2010870288, "AuId", 100);
            //int u = 0;
            Console.ReadKey();
        }

        static ArrayList SearchPath(Obj dept, Obj dest)
        {
            // 返回一个ArrayList，每个元素也是一个ArrayList，记录路径上的各个节点Obj
            ArrayList ret = new ArrayList();
            //ArrayList tempList1 = Request(dept);
            //ArrayList tempList2 = Request(dest);
            Obj temp_s1;
            Out0 temp_r;
            Out0 dept_detail = Request(dept);
            Out0 dest_detail = Request(dest);
            if (dept_detail.entities == null || dest_detail.entities == null)
            {
                return new ArrayList();
            }
            ArrayList inside_detail = new ArrayList();
            if (dept_detail.entities[0].RId != null)
            {
                foreach (Int64 RId in dept_detail.entities[0].RId)
                {
                    Obj a = new Obj("Paper", RId);
                    Out0 a_detail = Request(a);
                    if (a_detail.entities.Length > 0)
                        inside_detail.Add(a_detail.entities[0]);
                }
            }

            //1-hop

            if (dept.Type == "Paper")
            {
                if (dest.Type == "Paper")
                {
                    //ID -> ID
                    if (dept_detail.entities[0].RId != null)
                    {
                        foreach (Int64 RId in dept_detail.entities[0].RId)
                        {
                            if (RId == dest.ID)
                            {
                                ArrayList temp = new ArrayList();
                                temp.Add(dept.ID);
                                temp.Add(dest.ID);
                                ret.Add(temp);
                            }
                        }
                    }
                }
                else
                {
                    //ID -> AuthorID
                    if (dept_detail.entities[0].AA != null)
                    {
                        foreach (AAClass author in dept_detail.entities[0].AA)
                        {
                            if (dest.ID == author.AuId)
                            {
                                ArrayList temp = new ArrayList();
                                temp.Add(dept.ID);
                                temp.Add(dest.ID);
                                ret.Add(temp);
                            }
                        }
                    }
                }
            }
            else
            {
                if (dest.Type == "Paper")
                {
                    //AuthorID -> ID
                    foreach (IDClass IdC in dept_detail.entities)
                    {
                        if (dest.ID == IdC.Id)
                        {
                            ArrayList temp = new ArrayList();
                            temp.Add(dept.ID);
                            temp.Add(dest.ID);
                            ret.Add(temp);
                        }
                    }
                }
            }

            //2-hop
            if (dept.Type == "Paper")
            {
                if (dest.Type == "Paper")
                {
                    // ID -> ID -> ID
                    foreach (IDClass a_detail in inside_detail)
                    {
                        foreach (Int64 RId in a_detail.RId)
                        {
                            if (RId == dest.ID)
                            {
                                ArrayList temp = new ArrayList();
                                temp.Add(dept.ID);
                                temp.Add(a_detail.Id);
                                temp.Add(dest.ID);
                                ret.Add(temp);
                            }
                        }
                    }
                    // ID -> FId -> ID
                    if (dept_detail.entities[0].F != null && dest_detail.entities[0].F != null)
                    {
                        foreach (FClass F1 in dept_detail.entities[0].F)
                        {
                            foreach (FClass F2 in dest_detail.entities[0].F)
                            {
                                if (F1.FId == F2.FId)
                                {
                                    ArrayList temp = new ArrayList();
                                    temp.Add(dept.ID);
                                    temp.Add(F1.FId);
                                    temp.Add(dest.ID);
                                    ret.Add(temp);
                                }
                            }
                        }
                    }
                    // ID -> JId -> ID
                    if (dept_detail.entities[0].J != null && dest_detail.entities[0].J != null && dept_detail.entities[0].J.JId == dest_detail.entities[0].J.JId)
                    {
                        ArrayList temp = new ArrayList();
                        temp.Add(dept.ID);
                        temp.Add(dest_detail.entities[0].J.JId);
                        temp.Add(dest.ID);
                        ret.Add(temp);
                    }
                    // ID -> CId -> ID
                    if (dept_detail.entities[0].C != null && dest_detail.entities[0].C != null && dept_detail.entities[0].C.CId == dest_detail.entities[0].C.CId)
                    {
                        ArrayList temp = new ArrayList();
                        temp.Add(dept.ID);
                        temp.Add(dest_detail.entities[0].C.CId);
                        temp.Add(dest.ID);
                        ret.Add(temp);
                    }
                    // ID -> AuthorID -> ID
                    if (dest_detail.entities[0].AA != null && dest_detail.entities[0].AA != null)
                    {
                        foreach (AAClass A1 in dept_detail.entities[0].AA)
                        {
                            foreach (AAClass A2 in dest_detail.entities[0].AA)
                            {
                                if (A1.AuId == A2.AuId)
                                {
                                    ArrayList temp = new ArrayList();
                                    temp.Add(dept.ID);
                                    temp.Add(A1.AuId);
                                    temp.Add(dest.ID);
                                    ret.Add(temp);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // ID -> ID -> AuthorID
                    foreach (Int64 RId in dept_detail.entities[0].RId)
                    {
                        foreach (IDClass Id in dest_detail.entities)
                        {
                            if (RId == Id.Id)
                            {
                                ArrayList temp = new ArrayList();
                                temp.Add(dept.ID);
                                temp.Add(RId);
                                temp.Add(dest.ID);
                                ret.Add(temp);
                            }
                        }
                    }
                }
            }
            else
            {
                if (dest.Type == "Paper")
                {
                    // AuthorID -> ID -> ID
                    foreach (IDClass Id in dept_detail.entities)
                    {
                        foreach (Int64 RId in Id.RId)
                        {
                            if (RId == dest.ID)
                            {
                                ArrayList temp = new ArrayList();
                                temp.Add(dept.ID);
                                temp.Add(Id.Id);
                                temp.Add(dest.ID);
                                ret.Add(temp);
                            }
                        }
                    }
                }
                else
                {
                    // Author -> ID -> Author
                    foreach (IDClass Id in dept_detail.entities)
                    {
                        foreach (AAClass AA in Id.AA)
                        {
                            if (AA.AuId == dest.ID)
                            {
                                ArrayList temp = new ArrayList();
                                temp.Add(dept.ID);
                                temp.Add(Id.Id);
                                temp.Add(dest.ID);
                                ret.Add(temp);
                            }
                        }
                    }
                }
            }

            ////3-hop

            //if (dept.Type == "Paper")
            //{
            //    if (dest.Type == "Paper")
            //    {
            //        // ID -> ID -> ID -> ID
            //        foreach(IDClass a in inside_detail)
            //        {
            //            foreach(Int64 RId in a.RId)
            //            {
            //                temp_s1 = new Obj("Paper", RId);
            //                temp_r = Request(temp_s1);
            //                foreach(Int64 RId_2 in temp_r.entities[0].RId)
            //                {
            //                    if(RId_2 == dest.ID)
            //                    {
            //                        ArrayList temp = new ArrayList();
            //                        temp.Add(dept.ID);
            //                        temp.Add(a.Id);
            //                        temp.Add(RId);
            //                        temp.Add(dest.ID);
            //                        ret.Add(temp);
            //                    }
            //                }
            //            }
            //        }
            //        // ID -> ID -> CId -> ID
            //        foreach (IDClass a in inside_detail)
            //        {
            //            if(a.C.CId == dest_detail.entities[0].C.CId)
            //            {
            //                ArrayList temp = new ArrayList();
            //                temp.Add(dept.ID);
            //                temp.Add(a.Id);
            //                temp.Add(a.C.CId);
            //                temp.Add(dest.ID);
            //                ret.Add(temp);
            //            }
            //        }
            //        // ID -> ID -> JId -> ID
            //        foreach (IDClass a in inside_detail)
            //        {
            //            if (a.J.JId == dest_detail.entities[0].J.JId)
            //            {
            //                ArrayList temp = new ArrayList();
            //                temp.Add(dept.ID);
            //                temp.Add(a.Id);
            //                temp.Add(a.J.JId);
            //                temp.Add(dest.ID);
            //                ret.Add(temp);
            //            }
            //        }
            //        // ID -> ID -> FId -> ID
            //        foreach (IDClass a in inside_detail)
            //        {
            //            foreach(FClass F1 in a.F)
            //            {
            //                foreach(FClass F2 in dest_detail.entities[0].F)
            //                {
            //                    if(F1.FId == F2.FId)
            //                    {
            //                        ArrayList temp = new ArrayList();
            //                        temp.Add(dept.ID);
            //                        temp.Add(a.Id);
            //                        temp.Add(F1.FId);
            //                        temp.Add(dest.ID);
            //                        ret.Add(temp);
            //                    }
            //                }
            //            }
            //        }
            //        // ID -> ID -> AuId -> ID
            //        foreach (IDClass a in inside_detail)
            //        {
            //            foreach (AAClass A1 in a.AA)
            //            {
            //                foreach (AAClass A2 in dest_detail.entities[0].AA)
            //                {
            //                    if (A1.AuId == A2.AuId)
            //                    {
            //                        ArrayList temp = new ArrayList();
            //                        temp.Add(dept.ID);
            //                        temp.Add(a.Id);
            //                        temp.Add(A1.AuId);
            //                        temp.Add(dest.ID);
            //                        ret.Add(temp);
            //                    }
            //                }
            //            }
            //        }
            //        // ID -> CId -> ID -> ID
            //        temp_s1 = new Obj("Conference", dept_detail.entities[0].C.CId);
            //        temp_r = Request(temp_s1);
            //        foreach (IDClass Id in temp_r.entities)
            //        {
            //            foreach(Int64 RId in Id.RId)
            //            {
            //                if(RId == dest.ID)
            //                {
            //                    ArrayList temp = new ArrayList();
            //                    temp.Add(dept.ID);
            //                    temp.Add(dept_detail.entities[0].C.CId);
            //                    temp.Add(Id.Id);
            //                    temp.Add(dest.ID);
            //                    ret.Add(temp);
            //                }
            //            }
            //        }
            //        // ID -> JId => ID -> ID
            //        temp_s1 = new Obj("Journal", dept_detail.entities[0].J.JId);
            //        temp_r = Request(temp_s1);
            //        foreach (IDClass Id in temp_r.entities)
            //        {
            //            foreach (Int64 RId in Id.RId)
            //            {
            //                if (RId == dest.ID)
            //                {
            //                    ArrayList temp = new ArrayList();
            //                    temp.Add(dept.ID);
            //                    temp.Add(dept_detail.entities[0].J.JId);
            //                    temp.Add(Id.Id);
            //                    temp.Add(dest.ID);
            //                    ret.Add(temp);
            //                }
            //            }
            //        }
            //        // ID -> AuID -> ID -> ID
            //        foreach(AAClass author in dept_detail.entities[0].AA)
            //        {
            //            temp_s1 = new Obj("Author", author.AuId);
            //            temp_r = Request(temp_s1);
            //            foreach (IDClass Id in temp_r.entities)
            //            {
            //                foreach (Int64 RId in Id.RId)
            //                {
            //                    if (RId == dest.ID)
            //                    {
            //                        ArrayList temp = new ArrayList();
            //                        temp.Add(dept.ID);
            //                        temp.Add(author.AuId);
            //                        temp.Add(Id.Id);
            //                        temp.Add(dest.ID);
            //                        ret.Add(temp);
            //                    }
            //                }
            //            }
            //        }
            //        // ID -> FId -> ID -> ID
            //        foreach (FClass F in dept_detail.entities[0].F)
            //        {
            //            temp_s1 = new Obj("Author", F.FId);
            //            temp_r = Request(temp_s1);
            //            foreach (IDClass Id in temp_r.entities)
            //            {
            //                foreach (Int64 RId in Id.RId)
            //                {
            //                    if (RId == dest.ID)
            //                    {
            //                        ArrayList temp = new ArrayList();
            //                        temp.Add(dept.ID);
            //                        temp.Add(F.FId);
            //                        temp.Add(Id.Id);
            //                        temp.Add(dest.ID);
            //                        ret.Add(temp);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        // ID -> ID -> ID -> Author
            //        foreach(IDClass ID1 in inside_detail)
            //        {
            //            foreach(Int64 RId in ID1.RId)
            //            {
            //                foreach(IDClass ID2 in dest_detail.entities)
            //                {
            //                    if(RId == ID2.Id)
            //                    {
            //                        ArrayList temp = new ArrayList();
            //                        temp.Add(dept.ID);
            //                        temp.Add(ID1.Id);
            //                        temp.Add(RId);
            //                        temp.Add(dest.ID);
            //                        ret.Add(temp);
            //                    }
            //                }
            //            }
            //        }
            //        // ID -> FId -> ID -> Author
            //        foreach(IDClass Id1 in dest_detail.entities)
            //        {
            //            foreach(FClass F1 in dept_detail.entities[0].F)
            //            {
            //                foreach(FClass F2 in Id1.F)
            //                {
            //                    if(F1.FId == F2.FId)
            //                    {
            //                        ArrayList temp = new ArrayList();
            //                        temp.Add(dept.ID);
            //                        temp.Add(F1.FId);
            //                        temp.Add(Id1.Id);
            //                        temp.Add(dest.ID);
            //                        ret.Add(temp);
            //                    }
            //                }
            //            }
            //        }
            //        // ID -> CId -> ID -> Author
            //        foreach (IDClass Id1 in dest_detail.entities)
            //        {
            //            if(Id1.C == dept_detail.entities[0].C)
            //            {
            //                ArrayList temp = new ArrayList();
            //                temp.Add(dept.ID);
            //                temp.Add(Id1.C);
            //                temp.Add(Id1.Id);
            //                temp.Add(dest.ID);
            //                ret.Add(temp);
            //            }
            //        }
            //        // ID -> JId -> ID -> Author
            //        foreach (IDClass Id1 in dest_detail.entities)
            //        {
            //            if (Id1.J == dept_detail.entities[0].J)
            //            {
            //                ArrayList temp = new ArrayList();
            //                temp.Add(dept.ID);
            //                temp.Add(Id1.J);
            //                temp.Add(Id1.Id);
            //                temp.Add(dest.ID);
            //                ret.Add(temp);
            //            }
            //        }
            //        // ID -> Author -> ID -> Author
            //        foreach (IDClass Id1 in dest_detail.entities)
            //        {
            //            foreach (AAClass A1 in dept_detail.entities[0].AA)
            //            {
            //                foreach (AAClass A2 in Id1.AA)
            //                {
            //                    if (A1.AuId == A2.AuId)
            //                    {
            //                        ArrayList temp = new ArrayList();
            //                        temp.Add(dept.ID);
            //                        temp.Add(A1.AuId);
            //                        temp.Add(Id1.Id);
            //                        temp.Add(dest.ID);
            //                        ret.Add(temp);
            //                    }
            //                }
            //            }
            //        }
            //        // ID -> Author -> AuthorField -> Author
            //        foreach(AAClass A1 in dept_detail.entities[0].AA)
            //        {
            //            temp_s1 = new Obj("Author", A1.AuId);
            //            temp_r = Request(temp_s1);
            //            ArrayList src_author = new ArrayList();
            //            foreach(IDClass Id in temp_r.entities)
            //            {
            //                foreach(AAClass A2 in Id.AA)
            //                {
            //                    if(A2.AuId == A1.AuId)
            //                    {

            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //    if (dest.Type == "Paper")
            //    {

            //    }
            //    else
            //    {

            //    }
            //}

            return ret;
        }

        static bool isType(string s)
        {
            switch (s)
            {
                case "Id":
                    return true;
                case "RId":
                    return true;
                case "AfId":
                    return true;
                case "AuId":
                    return true;
                case "FId":
                    return true;
                case "JId":
                    return true;
                case "CId":
                    return true;
                case "logprob":
                    return true;
                default:
                    return false;
            }
        }


        static string GetRequest(string expression, string attributes, int count)
        {
            Console.WriteLine("API Request started");
            string requestUrl = "https:" + "//oxfordhk.azure-api.net/academic/v1.0/evaluate?expr=" + expression
                + "&count=" + count.ToString() + "&attributes=" + attributes
                + "&subscription-key=f7cc29509a8443c5b3a5e56b0e38b5a6";

            HttpWebRequest myRequest = WebRequest.Create(requestUrl) as HttpWebRequest;

            myRequest.ServicePoint.Expect100Continue = false;
            myRequest.ServicePoint.UseNagleAlgorithm = false;
            myRequest.Headers.Clear();  //清除http请求头信息
            myRequest.Timeout = 300000;   //超时时间
            myRequest.Method = "GET";  //默认GET方式提交
            myRequest.ContentType = "text/html";

            HttpWebResponse myResponse = myRequest.GetResponse() as HttpWebResponse;
            string response = string.Empty;
            using (StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8))
            {
                response = reader.ReadToEnd();
            }
            Console.WriteLine("API Request finished");
            return response;
        }

        // 将Bing返回的 json 字符串解析为由 Obj 元素构成的 ArrayList
        static Out0 Request(Obj obj)
        {
            Console.WriteLine("Request Obj:   Type: " + obj.Type + ",   ID: " + obj.ID.ToString());

            string req = "";
            string attr = "";
            switch (obj.Type)
            {
                case "Paper":
                    req = "Id=" + obj.ID.ToString();
                    attr = "Id%2cAA.AuId%2cF.FId%2cRId%2cJ.JId%2cC.CId";
                    break;
                case "Author":
                    req = "Composite(AA.AuId=" + obj.ID.ToString() + ")";
                    attr = "Id%2cAA.AfId";
                    break;
                case "AuthorField":
                    req = "Composite(AA.AfId=" + obj.ID.ToString() + ")";
                    attr = "AA.AuId";
                    break;
                case "Field":
                    req = "Composite(F.FId=" + obj.ID.ToString() + ")";
                    attr = "Id%2cAA.AuId%2cAA.AfId%2cF.FId%2cRId%2cJ.JId%2cC.CId";
                    break;
                case "Conference":
                    req = "Composite(C.CId=" + obj.ID.ToString() + ")";
                    attr = "Id";
                    break;
                case "Journal":
                    req = "Composite(J.JId=" + obj.ID.ToString() + ")";
                    attr = "Id";
                    break;
                default:
                    return new Out0();
            }

            string retAPI = GetRequest(req, attr, 500);
            //Console.WriteLine(retAPI);

            Out0 p = new Out0();
            MemoryStream stream = new MemoryStream(Encoding.Default.GetBytes(retAPI));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Out0));
            Out0 b = (Out0)ser.ReadObject(stream);

            Console.WriteLine("Request finished. Return count: " + b.entities.Length.ToString());
            return b;
        }

        static Out0 nRequest(string expr, string attr, int count)
        {
            string retAPI = GetRequest(expr, attr, count);
            MemoryStream stream = new MemoryStream(Encoding.Default.GetBytes(retAPI));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Out0));
            Out0 b = (Out0)ser.ReadObject(stream);

            Console.WriteLine("Request finished. Return count: " + b.entities.Length.ToString());
            return b;
        }

        static ArrayList nSearch(Obj dept, Obj dest)
        {
            Console.WriteLine("nSearch");
            ArrayList ret = new ArrayList();
            try
            {
                if (dept.Type == "Paper")
                {
                    if (dest.Type == "Paper")
                    {
                        // Paper to Paper
                        // 始末节点的请求统一进行，如需要更多属性请直接在此添加
                        Out0 deptOut = nRequest("Id=" + dept.ID.ToString(), "Id%2cRId%2cAA.AuId", 10000);
                        Out0 destOut = nRequest("Id=" + dest.ID.ToString(), "AA.AuId%2cF.FId%2cJ.JId%2cC.CId", 10000);

                        Console.WriteLine("fwafwaefds");
                        // Paper -> Paper
                        if (deptOut.entities[0].RId != null)
                        {
                            foreach(Int64 a in deptOut.entities[0].RId)
                            {
                                
                                if (a == dest.ID)
                                {
                                    
                                    ArrayList temp = new ArrayList();
                                    temp.Add(dept);
                                    temp.Add(dest);
                                    ret.Add(temp);
                                    break;
                                }
                            }
                        }

                        // Paper -> Paper -> Paper && Paper -> Paper -> Paper-> Paper && Paper -> Paper -> Author -> Paper
                        if (deptOut.entities[0].RId != null)
                        {
                            foreach(Int64 a in deptOut.entities[0].RId)
                            {
                                Out0 aOut = nRequest("Id=" + a.ToString(), "RId%2cAA.AuId%2cJ.JId%2cC.CId%2cF.FId", 10000);

                                if (aOut.entities[0].J != null && destOut.entities[0].J != null)
                                {
                                    if(aOut.entities[0].J.JId == destOut.entities[0].J.JId)
                                    {
                                        ArrayList temp = new ArrayList();
                                        temp.Add(dept);
                                        Obj _mid1 = new Obj("Paper", a);
                                        temp.Add(_mid1);
                                        Obj _mid2 = new Obj("Journal", aOut.entities[0].J.JId);
                                        temp.Add(_mid2);
                                        temp.Add(dest);
                                        ret.Add(temp);
                                        break;
                                    }
                                }

                                if (aOut.entities[0].C != null && destOut.entities[0].C != null)
                                {
                                    if (aOut.entities[0].C.CId == destOut.entities[0].C.CId)
                                    {
                                        ArrayList temp = new ArrayList();
                                        temp.Add(dept);
                                        Obj _mid1 = new Obj("Paper", a);
                                        temp.Add(_mid1);
                                        Obj _mid2 = new Obj("Conference", aOut.entities[0].C.CId);
                                        
                                        temp.Add(_mid2);
                                        temp.Add(dest);
                                        ret.Add(temp);
                                        break;
                                    }
                                }

                                if (aOut.entities[0].F != null && destOut.entities[0].F != null)
                                {
                                    foreach (FClass t1 in aOut.entities[0].F)
                                    {
                                        foreach(FClass t2 in destOut.entities[0].F)
                                        {
                                            Int64 m1 = t1.FId;
                                            Int64 m2 = t2.FId;
                                            if (m1 == m2)
                                            {
                                                ArrayList temp = new ArrayList();
                                                temp.Add(dept);
                                                Obj _mid1 = new Obj("Paper", a);
                                                temp.Add(_mid1);
                                                Obj _mid2 = new Obj("Field", m1);
                                                temp.Add(_mid2);
                                                temp.Add(dest);
                                                ret.Add(temp);
                                                break;
                                            }
                                        }
                                    }
                                   
                                }


                                if (aOut.entities[0].AA != null)
                                {
                                    foreach(AAClass b in aOut.entities[0].AA)
                                    {
                                        Out0 bOut = nRequest("Composite(AA.AuId=" + b.AuId.ToString() + ")", "Id", 10000);
                                        if (bOut.entities != null)
                                        {
                                            foreach(IDClass c in bOut.entities)
                                            {
                                                if (c.Id == dest.ID)
                                                {
                                                    ArrayList temp = new ArrayList();
                                                    temp.Add(dept);
                                                    Obj _mid1 = new Obj("Paper", a);
                                                    temp.Add(_mid1);
                                                    Obj _mid2 = new Obj("Author", b.AuId);
                                                    temp.Add(_mid2);
                                                    temp.Add(dest);
                                                    ret.Add(temp);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (aOut.entities[0].RId != null)
                                {
                                    foreach(Int64 b in aOut.entities[0].RId)
                                    {
                                        if (b == dest.ID)
                                        {
                                            ArrayList temp = new ArrayList();
                                            temp.Add(dept);
                                            Obj _mid = new Obj("Paper", a);
                                            temp.Add(_mid);
                                            temp.Add(dest);
                                            ret.Add(temp);
                                            break;
                                        }
                                        else
                                        {
                                            Out0 bOut = nRequest("Id=" + b.ToString(), "RId", 10000);
                                            if (bOut.entities[0].RId != null)
                                            {
                                                foreach(Int64 c in bOut.entities[0].RId)
                                                {
                                                    if (c == dest.ID)
                                                    {
                                                        ArrayList temp = new ArrayList();
                                                        temp.Add(dept);
                                                        Obj _mid1 = new Obj("Paper", a);
                                                        temp.Add(_mid1);
                                                        Obj _mid2 = new Obj("Paper", b);
                                                        temp.Add(_mid2);
                                                        temp.Add(dest);
                                                        ret.Add(temp);
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // Paper -> Author -> Paper && Paper -> Author -> Paper -> Paper
                        if(deptOut.entities[0].AA!=null && destOut.entities[0].AA != null)
                        {
                            foreach(AAClass a in deptOut.entities[0].AA)
                            {
                                foreach(AAClass b in destOut.entities[0].AA)
                                {
                                    if(a.AuId == b.AuId)
                                    {
                                        ArrayList temp = new ArrayList();
                                        temp.Add(dept);
                                        Obj _temp = new Obj("Author", a.AuId);
                                        temp.Add(_temp);
                                        temp.Add(dest);
                                        ret.Add(temp);
                                        break;
                                    }
                                }
                                Out0 aOut = nRequest("Composite(AA.AuId=" + a.AuId.ToString() + ")", "Id%2cRId", 10000);
                                if (aOut.entities != null)
                                {
                                    foreach( IDClass b in aOut.entities)
                                    {
                                        if (b.RId != null)
                                        {
                                            foreach(Int64 c in b.RId)
                                            {
                                                if (c == dest.ID)
                                                {
                                                    ArrayList temp = new ArrayList();
                                                    temp.Add(dept);
                                                    Obj _mid1 = new Obj("Author", a.AuId);
                                                    temp.Add(_mid1);
                                                    Obj _mid2 = new Obj("Paper", b.Id);
                                                    temp.Add(_mid2);
                                                    temp.Add(dest);
                                                    ret.Add(temp);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }


                }
                    else
                    {
                        // Paper to Author
                    }
                }
                else
                {
                    if (dest.Type == "Paper")
                    {
                        // Author to Paper
                    }
                    else
                    {
                        // Author to Author
                    }
                }
            }
            catch
            {
                return ret;
            }
            return ret;
        }

    }

}
