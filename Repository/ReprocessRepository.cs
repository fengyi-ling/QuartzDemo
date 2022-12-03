using QuartzDemo.Domain;

namespace QuartzDemo.Repository;

public class ReprocessRepository : IReprocessRepository
{
    private readonly HashSet<Reprocess> _collection = new()
    {
        new Reprocess(1, "{\"id\":\"1\",\"name\":\"BlueApple\"}", 0, false, "apple"),
        new Reprocess(2, "{\"id\":\"2\",\"name\":\"YellowApple\"}", 4, false, "apple"),
        new Reprocess(3, "{\"id\":\"3\",\"name\":\"BadApple\"}", 4, true, "apple"),
        new Reprocess(4, "{\"id\":\"4\",\"name\":\"GoodApple\"}", 5, false, "apple"),
        new Reprocess(5, "{\"id\":\"5\",\"name\":\"NoNameApple\"}", 1, true, "apple"),

        new Reprocess(10, "{\"id\":\"10\",\"name\":\"BlueOrange\"}", 0, false, "orange"),
        new Reprocess(11, "{\"id\":\"11\",\"name\":\"YellowOrange\"}", 4, true, "orange"),
        new Reprocess(12, "{\"id\":\"12\",\"name\":\"BadOrange\"}", 4, false, "orange"),
        new Reprocess(13, "{\"id\":\"13\",\"name\":\"GoodOrange\"}", 5, false, "orange"),
        new Reprocess(14, "{\"id\":\"14\",\"name\":\"NoNameOrange\"}", 1, true, "orange"),

        new Reprocess(20, "{\"id\":\"20\",\"name\":\"BlueBanana\"}", 0, false, "banana"),
        new Reprocess(21, "{\"id\":\"21\",\"name\":\"YellowBanana\"}", 4, true, "banana"),
        new Reprocess(22, "{\"id\":\"22\",\"name\":\"BadBanana\"}", 4, false, "banana"),
        new Reprocess(23, "{\"id\":\"23\",\"name\":\"GoodBanana\"}", 5, false, "banana"),
        new Reprocess(24, "{\"id\":\"24\",\"name\":\"NoNameBanana\"}", 1, true, "banana")
    };

    public ICollection<Reprocess> FindAllByDomainAndStatus(string domain, bool status)
    {
        return _collection
            .Where(reprocess => reprocess.Domain == domain)
            .Where(reprocess => reprocess.Status == status)
            .ToHashSet();
    }

    public void UpdateStatusById(int id, bool status)
    {
        var reprocesses = _collection
            .First(reprocess => reprocess.Id == id);
        reprocesses.Status = status;
    }

    public void IncreaseOneRetryAttemptById(int id)
    {
        var reprocess = _collection
            .First(reprocess => reprocess.Id == id);
        reprocess.RetryAttempt += 1;
    }
}