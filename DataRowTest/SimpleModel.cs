using System;

namespace DataRowTest
{
    public class SimpleModel
    {
        #region 变量定义
        
        private int _id = -1;
        
        private string _title = String.Empty;
        
        private string _gotourl = String.Empty;
        
        private DateTime _stime = DateTime.Now;
        
        private DateTime _etime = DateTime.Now;
        
        private string _runHour = String.Empty;
        
        private string _beizhu = String.Empty;
        
        private int _state = -1;
        
        private short _clickType = -1;
        
        private string _apkurl = String.Empty;
        
        private string _packageName = String.Empty;
        
        private string _mD5 = String.Empty;
        
        private short _silence = -1;
        #endregion

        #region 构造函数

        ///<summary>
        ///
        ///</summary>
        public SimpleModel()
        {
        }
        ///<summary>
        ///
        ///</summary>
        public SimpleModel
        (
            int id,
            string title,
            string gotourl,
            DateTime stime,
            DateTime etime,
            string runHour,
            string beizhu,
            int state,
            short clickType,
            string apkurl,
            string packageName,
            string mD5,
            short silence
        )
        {
            _id = id;
            _title = title;
            _gotourl = gotourl;
            _stime = stime;
            _etime = etime;
            _runHour = runHour;
            _beizhu = beizhu;
            _state = state;
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
        
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        
        public string Gotourl
        {
            get { return _gotourl; }
            set { _gotourl = value; }
        }
        
        public DateTime Stime
        {
            get { return _stime; }
            set { _stime = value; }
        }
        
        public DateTime Etime
        {
            get { return _etime; }
            set { _etime = value; }
        }
        
        public string RunHour
        {
            get { return _runHour; }
            set { _runHour = value; }
        }
        
        public string Beizhu
        {
            get { return _beizhu; }
            set { _beizhu = value; }
        }
        
        public int State
        {
            get { return _state; }
            set { _state = value; }
        }
        
        public short ClickType
        {
            get { return _clickType; }
            set { _clickType = value; }
        }
        
        public string Apkurl
        {
            get { return _apkurl; }
            set { _apkurl = value; }
        }
        
        public string PackageName
        {
            get { return _packageName; }
            set { _packageName = value; }
        }
        
        public string MD5
        {
            get { return _mD5; }
            set { _mD5 = value; }
        }
        
        public short Silence
        {
            get { return _silence; }
            set { _silence = value; }
        }

        #endregion

    }
}
