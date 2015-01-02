using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace photoPuzzle.Model
{
    public class DBContext : DataContext
    {
        // Pass the connection string to the base class.
        public DBContext(string connectionString)
            : base(connectionString)
        { }

        // Specify a table for the players items.
        public Table<Scores> scores;
    }

    [Table]
    public class Scores : INotifyPropertyChanged, INotifyPropertyChanging
    {

        // Define ID: private field, public property, and database column.
        private int _playerId;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int PlayerId
        {
            get { return _playerId; }
            set
            {
                if (_playerId != value)
                {
                    NotifyPropertyChanging("PlayerId");
                    _playerId = value;
                    NotifyPropertyChanged("PlayerId");
                }
            }
        }

        // Define item name: private field, public property, and database column.
        private int _moves;

        [Column]
        public int Moves
        {
            get { return _moves; }
            set
            {
                if (_moves != value)
                {
                    NotifyPropertyChanging("Moves");
                    _moves = value;
                    NotifyPropertyChanged("Moves");
                }
            }
        }

        private DateTime date;
        [Column(CanBeNull = false, AutoSync = AutoSync.Always)]
        public DateTime Date
        {
            get { return date; }
            set
            {
                if (date != value)
                {
                    NotifyPropertyChanging("Date");
                    date = value;
                    NotifyPropertyChanged("Date");
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
