# Teszt riport

## Utolsó futás

| Adat | Érték |
|------|-------|
| Dátum | 2026-04-23 |
| Időpont | 13:34:17 → 13:34:39 (≈22 mp) |
| Környezet | Bence-Linux (lokál) |
| Framework | xUnit |
| Eredmény | ✅ **396 / 396 passed** |

---

## Suite összefoglaló

| Suite | Tesztek | Passed | Failed | Futási idő |
|-------|---------|--------|--------|------------|
| Unit (`Tests.Services.Units`) | 184 | 184 | 0 | ~2 mp |
| Integration (`Tests.Services.Integrations`) | 143 | 143 | 0 | ~15 mp |
| Controller (`Tests.Controllers`) | 69 | 69 | 0 | ~5 mp |
| **Összesen** | **396** | **396** | **0** | **~22 mp** |

---

## Futtatási parancsok

```bash
# Összes teszt
dotnet test

# TRX riport generálása
dotnet test --logger "trx;LogFileName=test_results.trx" --results-directory ./docs/04_quality/

# Csak unit
dotnet test --filter "FullyQualifiedName~Tests.Services.Units"

# Csak integrációs
dotnet test --filter "FullyQualifiedName~Tests.Services.Integrations"

# Csak controller
dotnet test --filter "FullyQualifiedName~Tests.Controllers"
```

---

## Unit tesztek részletei (184 db)

### ArticleService (18)
| Teszt                                                           | Eredmény    |
|-----------------------------------------------------------------|-------------|
| Create_ShouldSucceed_WhenAuthorExists                           | ✅          |
| Create_ShouldFail_WhenAuthorNotFound                            | ✅          |
| Create_ShouldPassSummarySections_WhenProvided                   | ✅          |
| Create_ShouldFail_WhenArticleNotFound                           | ✅          |
| Delete_ShouldSucceed_AndCallRepositoryDelete                    | ✅          |
| Delete_ShouldFail_WhenArticleNotFound                           | ✅          |
| Delete_ShouldSucceed_WhenUserIsAuthor                           | ✅          |
| Delete_ShouldFail_WhenUserIsNotAuthor                           | ✅          |
| Delete_ShouldFail_WhenUserNotFound                              | ✅          |
| Update_ShouldSucceed_AndCallRepositoryUpdate                    | ✅          |
| Update_ShouldFail_WhenArticleNotFound                           | ✅          |
| Update_ShouldUpdateSummarySections_WhenProvided                 | ✅          |
| GetArticleById_ShouldReturnArticle_WhenExists                   | ✅          |
| GetArticleById_ShouldReturnFailure_WhenNotFound                 | ✅          |
| GetArticleBySlug_ShouldReturnArticle_WithCorrectFields          | ✅          |
| GetArticleBySlug_ShouldReturnMiddleSections_WhenSummary         | ✅          |
| GetArticleBySlug_ShouldReturnEmptyMiddleSections_WhenNotSummary | ✅          |
| GetArticleBySlug_ShouldReturnFailure_WhenArticleNotFound        | ✅          |
| GetArticleBySlug_Summary_ShouldHaveSecondaryImageUrls           | ✅          |
| GetArticleBySlug_NonSummary_ShouldHaveNullSecondaryImageUrls    | ✅          |
| GetRecentArticles_ShouldReturnSortedByDateDescending            | ✅          |
| GetRecentArticles_ShouldReturnEmptyList_WhenNoArticles          | ✅          |

### AuthService (14)
| Teszt                                                | Eredmény   |
|------------------------------------------------------|------------|
| Register_ShouldSucceed_WhenCredentialsAreUnique      | ✅         |
| Register_ShouldFail_WhenUsernameAlreadyTaken         | ✅         |
| Register_ShouldFail_WhenEmailAlreadyTaken            | ✅         |
| Register_ShouldCallRepositoryCreate_Once             | ✅         |
| Login_ShouldSucceed_WhenCredentialsAreCorrect        | ✅         |
| Login_ShouldFail_WhenEmailNotFound                   | ✅         |
| Login_ShouldFail_WhenPasswordIsWrong                 | ✅         |
| Login_ShouldReturnToken_WithCorrectUserId            | ✅         |
| Logout_ShouldSucceed_AndSetIsLoggedInFalse           | ✅         |
| Logout_ShouldFail_WhenUserNotFound                   | ✅         |
| ChangePassword_ShouldSucceed_WhenPasswordsAreValid   | ✅         |
| ChangePassword_ShouldFail_WhenUserNotFound           | ✅         |
| ChangePassword_ShouldFail_WhenCurrentPasswordIsWrong | ✅         |
| ChangePassword_ShouldFail_WhenNewPasswordSameAsOld   | ✅         |

### ChampionshipService (7)
| Teszt                                                                        | Eredmény   |
|------------------------------------------------------------------------------|------------|
| CreateChampionship_ShouldSucceed_AndCallBothRepos                            | ✅         |
| CreateParticipations_ShouldSkip_WhenAlreadyExists                            | ✅         |
| CreateParticipations_ShouldFail_WhenDriverNotFound                           | ✅         |
| CreateParticipations_ShouldFail_WhenConstructorNotFound                      | ✅         |
| UpdateChampionshipStatus_ShouldSucceed_AndUpdateBothStatuses                 | ✅         |
| UpdateChampionshipStatus_ShouldFail_WhenDriversChampNotFound                 | ✅         |
| UpdateChampionshipStatus_ShouldFail_WhenConstructorsChampNotFound            | ✅         |
| GetAllChampionshipsBySeries_ShouldReturnJoinedRows_OrderedBySeasonDescending | ✅         |
| GetAllChampionshipsBySeries_ShouldReturnEmpty_WhenNoMatchingSeason           | ✅         |

### CommentService (8)
| Teszt                                                      | Eredmény       |
|------------------------------------------------------------|----------------|
| Create_ShouldSucceed_AndCallCommentRepoCreate              | ✅             |
| Create_ShouldSetReplyToCommentId_WhenProvided              | ✅             |
| Create_ShouldFail_WhenUserNotFound                         | ✅             |
| Create_ShouldFail_WhenArticleNotFound                      | ✅             |
| DeleteComment_ShouldSucceed_WhenNoReplies                  | ✅             |
| DeleteComment_ShouldFail_WhenCommentNotFound               | ✅             |
| DeleteComment_ShouldDeleteRepliesFirst_ThenParent          | ✅             |
| UpdateCommentsContent_ShouldSucceed_AndUpdateContent       | ✅             |
| UpdateCommentsContent_ShouldFail_WhenCommentNotFound       | ✅             |
| UpdateCommentsContent_ShouldFail_WhenArticleIdMismatch     | ✅             |
| UpdateCommentsVote_ShouldAddUpvote_WhenNoExistingVote      | ✅             |
| UpdateCommentsVote_ShouldRemoveVote_WhenSameVoteExists     | ✅             |
| UpdateCommentsVote_ShouldSwitchVote_WhenOppositeVoteExists | ✅             |
| UpdateCommentsVote_ShouldFail_WhenCommentNotFound          | ✅             |

### Többi service
A fennmaradó unit tesztek a következő service-eket fedik le, mind passed:
ConstructorsService, ContractService, DriverService, GrandPrixService, PollService,
ResultsService, SeriesService, StandingsService, UserService.

---

## Integrációs tesztek részletei (143 db)

Minden integrációs teszt EF Core in-memory adatbázison fut, izolált környezetben.
Mind a 143 teszt passed. Főbb lefedett területek:

| Terület                            | Tesztek száma   | 
|------------------------------------|-----------------|
| Auth (regisztráció, login, jelszó) | 12              |
| Article CRUD + lapozás             | 14              |
| Comment + reply + szavazás         | 9               |
| Driver + Constructor CRUD          | 16              |
| Championship + participáció        | 11              |
| GrandPrix + Results + Recalculate  | 14              |
| Standings (driver + constructor)   | 8               |
| Poll + szavazás                    | 10              |
| Brand, Circuit, Series, Contract   | 29              |

---

## Controller tesztek részletei (69 db)

HTTP szintű API contract tesztek, `WebApplicationFactory` segítségével.
Mind a 69 teszt passed. Főbb lefedett területek:

| Terület                                  | Tesztek száma   |
|------------------------------------------|-----------------|
| Auth (login, register, logout)           | 8               |
| User (getMe, updateInfo, changePassword) | 6               |
| Article endpoints                        | 7               |
| Driver endpoints                         | 6               |
| Constructor endpoints                    | 5               |
| Championship endpoints                   | 4               |
| GrandPrix endpoints                      | 4               |
| Poll + szavazás                          | 6               |
| Comment endpoints                        | 4               |
| Brand, Circuit, Series, Contract         | 19              |

---

## Coverage

Coverage mérés jelenleg nincs konfigurálva. Futtatható:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

A tesztek széles service-lefedettséget biztosítanak (minden service-hez van unit + integrációs
teszt), de pontos %-os mérés még nem készült.

---

## Ismert hiányosságok

| Hiányossá g                          | Indok                                                               |
|--------------------------------------|---------------------------------------------------------------------|
| Nincs e2e / frontend teszt (Angular) | Időkeret korlát; a controller tesztek a HTTP contract szintet fedik |
| Nincs CI pipeline                    | Még nem konfigurált; lokálisan teljes mértékben futtatható          |
| Coverage % nem mért                  | Eszköz telepítve, de riport nem generált                            |
| Fájlfeltöltés (avatar) nem tesztelt  | Infrastruktúra-függő, mock nélkül nehéz izolálni                    |

---

## Flaky tesztek

Jelenleg nem ismert flaky teszt. Az integrációs tesztek izolált in-memory DB-t használnak,
ezért nem érzékenyek párhuzamos futásra vagy teszt-sorrendtől függő állapotra.