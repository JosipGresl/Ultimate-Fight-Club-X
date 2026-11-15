using SQLite;
using Ultimate_Fight_Club_X.Models;

namespace Ultimate_Fight_Club_X.Services;

public class MemberDatabase
{
    private SQLiteAsyncConnection? _db;

    private async Task InitAsync()
    {
        if (_db is not null) return;

        var path = Path.Combine(FileSystem.AppDataDirectory, "members_v2.db3");
        _db = new SQLiteAsyncConnection(path);
        await _db.CreateTableAsync<Member>();
        await _db.CreateTableAsync<Member>();
        await _db.CreateTableAsync<Attendance>();
    }

    public async Task<List<Member>> GetMembersAsync()
    {
        await InitAsync();
        return await _db!.Table<Member>().ToListAsync();
    }

    public async Task AddMemberAsync(Member member)
    {
        await InitAsync();
        await _db!.InsertAsync(member);
    }
    public async Task<List<Member>> GetActiveMembersByGroupAsync(string group)
    {
        await InitAsync();
        var members = await _db!.Table<Member>().Where(m => m.Group == group).ToListAsync();
        return members.Where(m => m.LeaveDate is null).OrderBy(m => m.FullName).ToList();
    }

    public async Task<HashSet<int>> GetPresentMemberIdsAsync(DateTime date)
    {
        await InitAsync();
        var day = date.Date;
        var records = await _db!.Table<Attendance>().Where(a => a.Date == day).ToListAsync();
        return records.Select(a => a.MemberId).ToHashSet();
    }

    public async Task SetAttendanceAsync(int memberId, DateTime date, bool present)
    {
        await InitAsync();
        var day = date.Date;
        var existing = await _db!.Table<Attendance>()
            .Where(a => a.MemberId == memberId && a.Date == day)
            .FirstOrDefaultAsync();

        if (present && existing is null)
            await _db.InsertAsync(new Attendance { MemberId = memberId, Date = day });
        else if (!present && existing is not null)
            await _db.DeleteAsync(existing);
    }
}