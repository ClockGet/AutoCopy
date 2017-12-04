using System;

namespace DataRowTest
{
    public class AdResourceModel
    {
        #region 变量定义
        ///<summary>
        ///
        ///</summary>
        private int _id = -1;
        ///<summary>
        ///广告主ID
        ///</summary>
        private int _aduserID = -1;
        ///<summary>
        ///广告项目ID标识
        ///</summary>
        private int _operationID = -1;
        ///<summary>
        ///广告标题
        ///</summary>
        private string _title = String.Empty;
        ///<summary>
        ///广告跳转地址
        ///</summary>
        private string _gotourl = String.Empty;
        ///<summary>
        ///广告剩余流量
        ///</summary>
        private int _shengYuIp = -1;
        ///<summary>
        ///媒体最大投放流量（IP）
        ///</summary>
        private int _everydaymax = -1;
        ///<summary>
        ///广告开始日期（含）
        ///</summary>
        private DateTime _stime = DateTime.Now;
        ///<summary>
        ///广告终止日期（含）
        ///</summary>
        private DateTime _etime = DateTime.Now;
        ///<summary>
        ///广告投放的时间段，格式：01,02,03,.....，默认为all
        ///</summary>
        private string _runHour = String.Empty;
        ///<summary>
        ///
        ///</summary>
        private string _beizhu = String.Empty;
        ///<summary>
        ///
        ///</summary>
        private int _state = -1;
        ///<summary>
        ///计费数据调整（扣量比例，如果给站长扣量10%则此值为0.9）
        ///</summary>
        private double _pK = -1;
        ///<summary>
        ///
        ///</summary>
        private float _price_adv = -1;
        ///<summary>
        ///
        ///</summary>
        private float _price_web = -1;
        ///<summary>
        ///
        ///</summary>
        private string _pricedanwei = String.Empty;
        ///<summary>
        ///是否定向广告，0为否，1为是
        ///</summary>
        private short _bDirect = -1;
        ///<summary>
        ///定向广告定向区域编号组合
        ///</summary>
        private string _directAddress = String.Empty;
        ///<summary>
        ///今日广告有效数据
        ///</summary>
        private int _todayNums = -1;
        ///<summary>
        ///昨日广告有效数据
        ///</summary>
        private int _yesterdayNums = -1;
        ///<summary>
        ///今日广告加载PV1
        ///</summary>
        private int _todayPv1 = -1;
        ///<summary>
        ///昨日广告加载PV1
        ///</summary>
        private int _yesterdayPv1 = -1;
        ///<summary>
        ///区分终端广告，0,PC广告，1,安卓手机广告，2,苹果广告
        ///</summary>
        private int _osSelect = -1;
        ///<summary>
        ///弹窗广告专用字段,为0时表示第1，2ip计费段 ，为1时表示一二IP以外的计费段
        ///</summary>
        private int _tgroup = -1;
        ///<summary>
        ///
        ///</summary>
        private string _justdms = String.Empty;
        ///<summary>
        ///0:cpm,1:富媒体，2：对联，3贴片，4拉幕，5CPC
        ///</summary>
        private int _adType = -1;
        ///<summary>
        ///
        ///</summary>
        private int _zuoRiShengyuIp = -1;
        ///<summary>
        ///
        ///</summary>
        private float _price_Standard = -1;
        ///<summary>
        ///
        ///</summary>
        private string _tagSels = String.Empty;
        ///<summary>
        ///
        ///</summary>
        private decimal _everydaymaxTemp = -1;
        ///<summary>
        ///
        ///</summary>
        private int _classify = -1;
        ///<summary>
        ///
        ///</summary>
        private string _directionaltags = String.Empty;
        ///<summary>
        ///
        ///</summary>
        private int _jftype = 0;
        ///<summary>
        ///点击类型 1:跳转、2:下载
        ///</summary>
        private short _clickType = -1;
        ///<summary>
        ///APK下载地址
        ///</summary>
        private string _apkurl = String.Empty;
        ///<summary>
        ///包名
        ///</summary>
        private string _packageName = String.Empty;
        ///<summary>
        ///包的MD5值
        ///</summary>
        private string _mD5 = String.Empty;
        ///<summary>
        ///静默安装 0：否 1：是
        ///</summary>
        private short _silence = -1;
        #endregion

        #region 构造函数

        ///<summary>
        ///
        ///</summary>
        public AdResourceModel()
        {
        }
        ///<summary>
        ///
        ///</summary>
        public AdResourceModel
        (
            int id,
            int aduserID,
            int operationID,
            string title,
            string gotourl,
            int shengYuIp,
            int everydaymax,
            DateTime stime,
            DateTime etime,
            string runHour,
            string beizhu,
            int state,
            double pK,
            float price_adv,
            float price_web,
            string pricedanwei,
            short bDirect,
            string directAddress,
            int todayNums,
            int yesterdayNums,
            int todayPv1,
            int yesterdayPv1,
            int osSelect,
            int tgroup,
            string justdms,
            int adType,
            int zuoRiShengyuIp,
            float price_Standard,
            string tagSels,
            decimal everydaymaxTemp,
            int classify,
            string directionaltags,
            int jftype,
            short clickType,
            string apkurl,
            string packageName,
            string mD5,
            short silence
        )
        {
            _id = id;
            _aduserID = aduserID;
            _operationID = operationID;
            _title = title;
            _gotourl = gotourl;
            _shengYuIp = shengYuIp;
            _everydaymax = everydaymax;
            _stime = stime;
            _etime = etime;
            _runHour = runHour;
            _beizhu = beizhu;
            _state = state;
            _pK = pK;
            _price_adv = price_adv;
            _price_web = price_web;
            _pricedanwei = pricedanwei;
            _bDirect = bDirect;
            _directAddress = directAddress;
            _todayNums = todayNums;
            _yesterdayNums = yesterdayNums;
            _todayPv1 = todayPv1;
            _yesterdayPv1 = yesterdayPv1;
            _osSelect = osSelect;
            _tgroup = tgroup;
            _justdms = justdms;
            _adType = adType;
            _zuoRiShengyuIp = zuoRiShengyuIp;
            _price_Standard = price_Standard;
            _tagSels = tagSels;
            _everydaymaxTemp = everydaymaxTemp;
            _classify = classify;
            _directionaltags = directionaltags;
            _jftype = jftype;
            _clickType = clickType;
            _apkurl = apkurl;
            _packageName = packageName;
            _mD5 = mD5;
            _silence = silence;

        }
        #endregion

        #region 公共属性


        ///<summary>
        ///
        ///</summary>
        public int id
        {
            get { return _id; }
            set { _id = value; }
        }

        ///<summary>
        ///广告主ID
        ///</summary>
        public int AduserID
        {
            get { return _aduserID; }
            set { _aduserID = value; }
        }

        ///<summary>
        ///广告项目ID标识
        ///</summary>
        public int OperationID
        {
            get { return _operationID; }
            set { _operationID = value; }
        }

        ///<summary>
        ///广告标题
        ///</summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        ///<summary>
        ///广告跳转地址
        ///</summary>
        public string Gotourl
        {
            get { return _gotourl; }
            set { _gotourl = value; }
        }

        ///<summary>
        ///广告剩余流量
        ///</summary>
        public int ShengYuIp
        {
            get { return _shengYuIp; }
            set { _shengYuIp = value; }
        }

        ///<summary>
        ///媒体最大投放流量（IP）
        ///</summary>
        public int Everydaymax
        {
            get { return _everydaymax; }
            set { _everydaymax = value; }
        }

        ///<summary>
        ///广告开始日期（含）
        ///</summary>
        public DateTime Stime
        {
            get { return _stime; }
            set { _stime = value; }
        }

        ///<summary>
        ///广告终止日期（含）
        ///</summary>
        public DateTime Etime
        {
            get { return _etime; }
            set { _etime = value; }
        }

        ///<summary>
        ///广告投放的时间段，格式：01,02,03,.....，默认为all
        ///</summary>
        public string RunHour
        {
            get { return _runHour; }
            set { _runHour = value; }
        }

        ///<summary>
        ///
        ///</summary>
        public string Beizhu
        {
            get { return _beizhu; }
            set { _beizhu = value; }
        }

        ///<summary>
        ///
        ///</summary>
        public int State
        {
            get { return _state; }
            set { _state = value; }
        }

        ///<summary>
        ///计费数据调整（扣量比例，如果给站长扣量10%则此值为0.9）
        ///</summary>
        public double PK
        {
            get { return _pK; }
            set { _pK = value; }
        }

        ///<summary>
        ///
        ///</summary>
        public float Price_adv
        {
            get { return _price_adv; }
            set { _price_adv = value; }
        }

        ///<summary>
        ///
        ///</summary>
        public float Price_web
        {
            get { return _price_web; }
            set { _price_web = value; }
        }

        ///<summary>
        ///
        ///</summary>
        public string Pricedanwei
        {
            get { return _pricedanwei; }
            set { _pricedanwei = value; }
        }

        ///<summary>
        ///是否定向广告，0为否，1为是
        ///</summary>
        public short bDirect
        {
            get { return _bDirect; }
            set { _bDirect = value; }
        }

        ///<summary>
        ///定向广告定向区域编号组合
        ///</summary>
        public string DirectAddress
        {
            get { return _directAddress; }
            set { _directAddress = value; }
        }

        ///<summary>
        ///今日广告有效数据
        ///</summary>
        public int TodayNums
        {
            get { return _todayNums; }
            set { _todayNums = value; }
        }

        ///<summary>
        ///昨日广告有效数据
        ///</summary>
        public int YesterdayNums
        {
            get { return _yesterdayNums; }
            set { _yesterdayNums = value; }
        }

        ///<summary>
        ///今日广告加载PV1
        ///</summary>
        public int TodayPv1
        {
            get { return _todayPv1; }
            set { _todayPv1 = value; }
        }

        ///<summary>
        ///昨日广告加载PV1
        ///</summary>
        public int YesterdayPv1
        {
            get { return _yesterdayPv1; }
            set { _yesterdayPv1 = value; }
        }

        ///<summary>
        ///区分终端广告，0,PC广告，1,安卓手机广告，2,苹果广告
        ///</summary>
        public int OsSelect
        {
            get { return _osSelect; }
            set { _osSelect = value; }
        }

        ///<summary>
        ///弹窗广告专用字段,为0时表示第1，2ip计费段 ，为1时表示一二IP以外的计费段
        ///</summary>
        public int tgroup
        {
            get { return _tgroup; }
            set { _tgroup = value; }
        }

        ///<summary>
        ///
        ///</summary>
        public string justdms
        {
            get { return _justdms; }
            set { _justdms = value; }
        }

        ///<summary>
        ///0:cpm,1:富媒体，2：对联，3贴片，4拉幕，5CPC
        ///</summary>
        public int AdType
        {
            get { return _adType; }
            set { _adType = value; }
        }

        ///<summary>
        ///
        ///</summary>
        public int ZuoRiShengyuIp
        {
            get { return _zuoRiShengyuIp; }
            set { _zuoRiShengyuIp = value; }
        }

        ///<summary>
        ///
        ///</summary>
        public float Price_Standard
        {
            get { return _price_Standard; }
            set { _price_Standard = value; }
        }

        ///<summary>
        ///
        ///</summary>
        public string TagSels
        {
            get { return _tagSels; }
            set { _tagSels = value; }
        }

        ///<summary>
        ///
        ///</summary>
        public decimal EverydaymaxTemp
        {
            get { return _everydaymaxTemp; }
            set { _everydaymaxTemp = value; }
        }

        ///<summary>
        ///
        ///</summary>
        public int Classify
        {
            get { return _classify; }
            set { _classify = value; }
        }

        ///<summary>
        ///
        ///</summary>
        public string directionaltags
        {
            get { return _directionaltags; }
            set { _directionaltags = value; }
        }

        ///<summary>
        ///
        ///</summary>
        public int jftype
        {
            get { return _jftype; }
            set { _jftype = value; }
        }

        ///<summary>
        ///点击类型 1:跳转、2:下载
        ///</summary>
        public short ClickType
        {
            get { return _clickType; }
            set { _clickType = value; }
        }

        ///<summary>
        ///APK下载地址
        ///</summary>
        public string Apkurl
        {
            get { return _apkurl; }
            set { _apkurl = value; }
        }

        ///<summary>
        ///包名
        ///</summary>
        public string PackageName
        {
            get { return _packageName; }
            set { _packageName = value; }
        }

        ///<summary>
        ///包的MD5值
        ///</summary>
        public string MD5
        {
            get { return _mD5; }
            set { _mD5 = value; }
        }

        ///<summary>
        ///静默安装 0：否 1：是
        ///</summary>
        public short Silence
        {
            get { return _silence; }
            set { _silence = value; }
        }

        #endregion

    }
}
