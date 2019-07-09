using LitJson;

namespace DataItem
{

    public class AddressData
    {
        public string id;
        public string name;
        public string tel;
        public string province;
        public string city;
        public string area;
        public string address;
        public string zip_code;
        public string moren;
    }
    public class GoodsData
    {
        public string id;
        public string title;
        public string type_s;
        public string sj;
        public string price;
        public string price_second;
        public string content;
        public string url;
        public string type_id;
    }

    public class ShopCar
    {
        public string id;
        public string goods_id;
        public string goods_num;
        public string sj;
    }

    //用户数据
    public class UserData
    {
        public string zhifubao;
        public int createtime;
		public int lastlogin;
		public int status;
		public int cumulative;
		public string ip;
		public int cra;
		public string password2;
		public double dsb;
		public string avatar;
		public int sex;
		public long superior;
		public string nickname;
		public double sb;
		public string strlevel;
		public int direct;
		public long phone;
		public string password;
		public int id;
        public string bankcard;
        public string bank;
        public double totalpower;
    }

}