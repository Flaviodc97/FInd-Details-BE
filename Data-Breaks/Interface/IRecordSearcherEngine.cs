using Data_Breaks.Class;

namespace Data_Breaks.Interface
{
    public interface IRecordSearcherEngine
    {

        public void CreateIndex();
        public List<Record> Search(string name, string surname);


    }
}
