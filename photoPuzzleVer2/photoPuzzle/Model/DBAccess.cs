using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace photoPuzzle.Model
{
    public class DBAccess
    {
        DBContext appDB;

        //constructor
        public DBAccess(string DBString)
        {
            appDB = new DBContext(DBString);
        }

        //scores
        private ObservableCollection<Scores> _allScores;
        public ObservableCollection<Scores> AllScores
        {
            get { return _allScores; }
            set
            {
                _allScores = value;
                NotifyPropertyChanged("AllScores");
            }
        }

        public void LoadCollectionsFromDatabase()
        {
            var scoresInDB = from Scores newScore in appDB.scores
                             .OrderBy(g=>g.PlayerId)
                             select newScore;
            AllScores = new ObservableCollection<Scores>(scoresInDB);
        }

        public void AddScoresToDb(Scores newScore)
        {
            appDB.scores.InsertOnSubmit(newScore);
            SaveChangesToDB();
            Debug.WriteLine("Inserted in DB");
        }

        // Write changes in the data context to the database.
        public void SaveChangesToDB()
        {
            appDB.SubmitChanges();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
