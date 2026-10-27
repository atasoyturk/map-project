using System.Net.Http.Json;
using BackendApi.DTOs.Annotation;
using BackendApi.Entities.Team;
using BackendApi.Tests.Infrastructure;
using NetTopologySuite.Geometries;
using AnnotationEntity = BackendApi.Entities.Annotation.Annotation;

namespace BackendApi.Tests;

// Rule: admin sees all notes; everyone else sees their own notes, their team's notes and admin notes.
// An admin note is a note whose author currently has admin_access (via role or direct permission).
public sealed class AnnotationVisibilityTests : ApiTestBase
{
    public AnnotationVisibilityTests(ApiFixture fixture) : base(fixture) { }

    [Theory]
    [InlineData("admin",   "adminNote,permAdminNote,a1Note,bNote,loner1Note,loner2Note,moverOldNote")]
    [InlineData("memberA", "adminNote,permAdminNote,a1Note,moverOldNote")]
    [InlineData("memberB", "adminNote,permAdminNote,bNote")]
    [InlineData("loner1",  "adminNote,permAdminNote,loner1Note")]
    [InlineData("mover",   "adminNote,permAdminNote,bNote,moverOldNote")]
    public async Task GetAll_ReturnsVisibleNotes(string viewer, string expected)
    {
        var teamA = await CreateTeamAsync("A");
        var teamB = await CreateTeamAsync("B");

        var (adminId,     admin)   = await CreateUserAsync([Roles.Admin], teamId: teamB);
        var (permAdminId, _)       = await CreateUserAsync([], ["admin_access"]);
        var (a1Id,        _)       = await CreateUserAsync([Roles.Intern], teamId: teamA);
        var (_,           memberA) = await CreateUserAsync([Roles.Intern], teamId: teamA);
        var (bId,         memberB) = await CreateUserAsync([Roles.Intern], teamId: teamB);
        var (loner1Id,    loner1)  = await CreateUserAsync([Roles.Intern]);
        var (loner2Id,    _)       = await CreateUserAsync([Roles.Intern]);
        // "mover" wrote a note while in team A, then moved to team B.
        var (moverId,     mover)   = await CreateUserAsync([Roles.Intern], teamId: teamB);

        var notes = new Dictionary<string, (int UserId, int? TeamId)>
        {
            ["adminNote"]     = (adminId,     teamB),
            ["permAdminNote"] = (permAdminId, null),
            ["a1Note"]        = (a1Id,        teamA),
            ["bNote"]         = (bId,         teamB),
            ["loner1Note"]    = (loner1Id,    null),
            ["loner2Note"]    = (loner2Id,    null),
            ["moverOldNote"]  = (moverId,     teamA)
        };
        await WithDbAsync(async db =>
        {
            foreach (var (text, (userId, teamId)) in notes)
                db.Annotations.Add(new AnnotationEntity
                {
                    NoteText = text, UserId = userId, TeamId = teamId,
                    Geometry = new Point(29, 41) { SRID = 4326 }
                });
            await db.SaveChangesAsync();
        });

        var clients = new Dictionary<string, HttpClient>
        {
            ["admin"] = admin, ["memberA"] = memberA, ["memberB"] = memberB, ["loner1"] = loner1, ["mover"] = mover
        };
        var result = await clients[viewer].GetFromJsonAsync<List<AnnotationResponseDto>>("/api/annotation");

        Assert.Equal(expected.Split(',').Order(), result!.Select(a => a.NoteText).Order());
    }

    private async Task<int> CreateTeamAsync(string name)
    {
        var id = 0;
        await WithDbAsync(async db =>
        {
            var team = new Team { Name = name };
            db.Teams.Add(team);
            await db.SaveChangesAsync();
            id = team.Id;
        });
        return id;
    }
}
