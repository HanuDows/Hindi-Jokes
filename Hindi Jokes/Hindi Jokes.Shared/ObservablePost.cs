using HanuDowsFramework;
using System;
using System.ComponentModel;

namespace Hindi_Jokes
{
    /// <summary>
    /// This class encapsulates the actual Post object.
    /// This class is bound to the UI. We don't want to expose the real Post Object
    /// </summary>
    /// 
    public class ObservablePost : INotifyPropertyChanged
    {
        private string _title, _content, _metaData;
        private PostManager pm;
        private int index;

        private static ObservablePost instance;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public static ObservablePost getInstance()
        {
            if (instance == null)
            {
                instance = new ObservablePost();
            }
            return instance;
        }

        private ObservablePost()
        {
            pm = PostManager.getInstance();
            _title = "Restart the application";
            _metaData = "";
            _content = "Error occured during initialization, please restart the application";

            index = 0;
        }

        internal void Reset()
        {
            index = 0;
            if (pm.PostList.Count > 0)
            {
                readPost();
            }
            else
            {
                _title = "Restart the application";
                _metaData = "";
                _content = "Error occured during initialization, please restart the application";
            }

            NotifyDataChanged();
        }

        internal void NextPost()
        {
            if (index == pm.PostList.Count - 1)
            {
                Reset();
            }
            else
            {
                index++;
                readPost();
            }

            if (pm.PostList.Count - index <=3)
            {
                HanuDowsApplication.getInstance().ReadPostsFromDB(true);
            }
            
        }

        internal void PreviousPost()
        {
            if (index == 0)
            {
                index = pm.PostList.Count - 1;
            }
            else
            {
                index--;
            }

            readPost();
        }

        private void readPost()
        {
            Post post = pm.PostList[index];
            _title = post.PostTitle;
            _metaData = "Published On: " + post.PubDate;
            _content = post.ShareableContent;

            NotifyDataChanged();
        }

        public string Title
        {
            get { return _title; }
        }

        public string Content
        {
            get { return _content; }
        }

        public string MetaData
        {
            get { return _metaData; }
        }

        public int CurrentIndex
        {
            get { return index; }

            set {
                index = value;
                if (index >= pm.PostList.Count)
                {
                    Reset();
                }
                else
                {
                    readPost();
                }
            }
        }

        private void NotifyDataChanged()
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(String.Empty));
        }

    }
}
