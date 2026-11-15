using SQLite;
using Ultimate_Fight_Club_X.Models;

namespace Ultimate_Fight_Club_X.Services;

public class MemberDatabase
{
    private SQLiteAsyncConnection? _db;

    private async Task InitAsync()
    {
        if (_db is not null) return;

        var path = Path.Combine(FileSystem.AppDataDirectory, "members.db3");
        _db = new SQLiteAsyncConnection(path);
        await _db.CreateTableAsync<Member>();
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
}