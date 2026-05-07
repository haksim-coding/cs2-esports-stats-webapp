# Sitemap

The auth-friendly routes are defined in [Program.cs](Program.cs) before the default conventional route. The semantic team, player, and event detail routes are defined with attribute routing in [Controllers/TeamsController.cs](Controllers/TeamsController.cs), [Controllers/PlayersController.cs](Controllers/PlayersController.cs), and [Controllers/EventsController.cs](Controllers/EventsController.cs). The slug lookup logic lives in [Helpers/RouteSlugHelper.cs](Helpers/RouteSlugHelper.cs).

## Preferred public routes

| URL | Method | Controller.Action | View |
| --- | --- | --- | --- |
| `/` | GET | `HomeController.Index` | `Views/Home/Index.cshtml` |
| `/teams` | GET | `TeamsController.Index` | `Views/Teams/Index.cshtml` |
| `/team/{slug}` | GET | `TeamsController.DetailsBySlug(string slug)` | `Views/Teams/Details.cshtml` |
| `/players` | GET | `PlayersController.Index` | `Views/Players/Index.cshtml` |
| `/player/{slug}` | GET | `PlayersController.DetailsBySlug(string slug)` | `Views/Players/Details.cshtml` |
| `/events` | GET | `EventsController.Index` | `Views/Events/Index.cshtml` |
| `/event/{slug}` | GET | `EventsController.DetailsBySlug(string slug)` | `Views/Events/Details.cshtml` |
| `/forums` | GET | `ForumsController.Index` | `Views/Forums/Index.cshtml` |
| `/forums/create` | GET | `ForumsController.Create` | `Views/Forums/Create.cshtml` |
| `/forums/{id}` | GET | `ForumsController.Details(int id)` | `Views/Forums/Details.cshtml` |
| `/login` | GET, POST | `AuthController.Login` | `Views/Auth/Login.cshtml` |
| `/register` | GET, POST | `AuthController.Register` | `Views/Auth/Register.cshtml` |
| `/logout` | GET | `AuthController.Logout` | No view, redirects to Home |
| `/my-profile` | GET | `AuthController.Profile` | `Views/Auth/Profile.cshtml` |
| `/Home/Privacy` | GET | `HomeController.Privacy` | `Views/Home/Privacy.cshtml` |
| `/Home/Error` | GET | `HomeController.Error` | `Views/Home/Error.cshtml` |

## Form and action endpoints

| URL | Method | Controller.Action | View |
| --- | --- | --- | --- |
| `/forums/create` | POST | `ForumsController.Create(ForumCreateInputModel)` | Re-renders `Views/Forums/Create.cshtml` on validation errors |
| `/forums/comment` | POST | `ForumsController.Comment(ForumCommentInputModel)` | Re-renders `Views/Forums/Details.cshtml` on validation errors |
| `/favorites/toggleteam` | POST | `FavoritesController.ToggleTeam(int id)` | No view, redirects back |
| `/favorites/toggleplayer` | POST | `FavoritesController.TogglePlayer(int id)` | No view, redirects back |

## Legacy conventional aliases

The default route is still enabled in [Program.cs](Program.cs), so the older controller/action paths continue to resolve even though the app now uses the semantic URLs in navigation links. The forum details page also has an explicit attribute route at `/forums/{id}` on [Controllers/ForumsController.cs](Controllers/ForumsController.cs).

| URL | Method | Controller.Action | View |
| --- | --- | --- | --- |
| `/Home/Index` | GET | `HomeController.Index` | `Views/Home/Index.cshtml` |
| `/Home/Privacy` | GET | `HomeController.Privacy` | `Views/Home/Privacy.cshtml` |
| `/Teams/Index` | GET | `TeamsController.Index` | `Views/Teams/Index.cshtml` |
| `/Teams/Details/{id}` | GET | `TeamsController.Details(int id)` | `Views/Teams/Details.cshtml` |
| `/Players/Index` | GET | `PlayersController.Index` | `Views/Players/Index.cshtml` |
| `/Players/Details/{id}` | GET | `PlayersController.Details(int id)` | `Views/Players/Details.cshtml` |
| `/Events/Index` | GET | `EventsController.Index` | `Views/Events/Index.cshtml` |
| `/Events/Details/{id}` | GET | `EventsController.Details(int id)` | `Views/Events/Details.cshtml` |
| `/Forums/Index` | GET | `ForumsController.Index` | `Views/Forums/Index.cshtml` |
| `/Forums/Create` | GET, POST | `ForumsController.Create` | `Views/Forums/Create.cshtml` |
| `/Forums/Details/{id}` | GET | `ForumsController.Details(int id)` | `Views/Forums/Details.cshtml` |
| `/Auth/Login` | GET, POST | `AuthController.Login` | `Views/Auth/Login.cshtml` |
| `/Auth/Register` | GET, POST | `AuthController.Register` | `Views/Auth/Register.cshtml` |
| `/Auth/Logout` | GET | `AuthController.Logout` | No view, redirects to Home |
| `/Auth/Profile` | GET | `AuthController.Profile` | `Views/Auth/Profile.cshtml` |
