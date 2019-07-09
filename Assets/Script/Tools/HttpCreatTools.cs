using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Tools
{

    public class HttpCreatTools
    {
        public static Http CreatHttp(string str)
        {
            Http http = MessageManager.GetMessageManager.Http.Get(str);
            
            if (http == null)
                http = new Http(str);
            else
                http.Clear();

            return http;
        }     
    }

    public class ListCreatTools
    {
        static Dic<string, ListGroup> Body = new Dic<string, ListGroup>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listName">缓存数据的标签</param>
        /// <param name="item">实例对象</param>
        /// <param name="Parent">实例对象父物体</param>
        /// <param name="Clear">是否自动清空当前列表，设置为true时手动清空将无效</param>
        /// <returns></returns>
        public static ListGroup Creat(string listName, GameObject item, Transform Parent, bool Clear = false)
        {
            ListGroup obj = Body.Get(listName);
            if (obj == null)
            {
                obj = new ListGroup(item, Parent);              
                Body.Add(listName, obj);
            }
            if (Clear)
                obj.Clear();
            return obj;
        }

        public static void Clear()
        {
            Body.Clear();
        }
    }


}