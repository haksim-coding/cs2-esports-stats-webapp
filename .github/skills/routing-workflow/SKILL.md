---
name: routing-workflow
description: "Use when adding custom routing, attribute routes, route constraints, controller route prefixes, URL cleanup, or sitemap/routing documentation."
user-invocable: true
---

# Routing Workflow

Use this skill when you need to change how URLs map to controllers, actions, or views, or when you need to document the URL structure.

## What this skill covers
- Attribute routing with `[Route]`
- Custom conventional routes in `Program.cs`
- Route constraints and optional parameters
- Friendly URLs for list and detail pages
- Mapping routes to a sitemap or routing document
- Verifying that existing views still match the new URLs

## How to use it
Invoke this skill when you are about to touch any of the following:
- `app.MapControllerRoute(...)`
- `[Route(...)]` on controllers or actions
- route parameters like `{id?}` or `{slug}`
- custom URL patterns for lists, details, create, login, or forum actions
- `sitemap.md` or other route documentation

## Recommended workflow
1. List the URLs the app should expose.
2. Decide whether the route belongs in `Program.cs` or as attribute routing.
3. Add the route and keep the controller/action names consistent.
4. Check every linked view and action parameter for the new URL shape.
5. Add or update any route constraints if the segment should be numeric, slug-like, or optional.
6. Document the final mapping in `sitemap.md`.
7. Test the routes in the browser and fix any broken links.

## Practical rules
- Use attribute routing when you want a clear, action-specific URL.
- Use conventional routing when the pattern is shared across several actions.
- Keep controller action names readable; route text can be more user-friendly than the method name.
- If you add a custom detail route, make sure redirects and links generate the same URL shape.
- For the lab, aim for at least four non-default custom routes so the change is clearly visible.

## Validation checklist
- Start the app and browse the new URLs directly.
- Confirm `Url.Action(...)` and redirects still resolve correctly.
- Verify list/detail pages still render their expected views.
- Update `sitemap.md` after the routes are stable.

## When to stop and inspect
- If a route collides with an existing action or controller path
- If model binding breaks because a route segment type changed
- If the default route still reaches the same page and the custom route is not actually being used
