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

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

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

        public void setPost(Post post)
        {
            _title = post.PostTitle;
            _metaData = "Published On: " + post.PubDate;
            _content = post.ShareableContent;
            DataChanged();
        }

        public ObservablePost(Post post)
        {
            setPost(post);
        }

        private void DataChanged()
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(String.Empty));
        }
    }
}
