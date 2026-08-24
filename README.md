# D365 Form Tracking POC

A minimal ASP.NET Core static-file host for testing whether **visiting**
a D365 Customer Insights - Journeys marketing form (without submitting it)
gets tracked as its own event.

This project doesn't call any Dynamics APIs. It's just a real local web
server you can tunnel to a public HTTPS URL, so D365 has a domain it's
willing to render/track the form against.

## Requirements

- Admin access in D365 Customer Insights - Journeys to:
  - Publish a marketing form and open its **Form hosting** tab
  - Add a domain to the **allowed domains for external form hosting** list
- Either:
  - **Option A (no local install at all):** a free [GitHub](https://github.com/signup)
    account and a browser — uses GitHub Codespaces
  - **Option B (if you can run local software):** [.NET 8 SDK](https://dotnet.microsoft.com/download)
    plus a tunneling tool like [ngrok](https://ngrok.com/download)

---

## Option A — GitHub Codespaces (no local install required)

This runs the whole app in the cloud, in your browser, and gives you a
real public HTTPS URL. Nothing gets installed on your machine.

1. Go to [github.com](https://github.com) and sign in (or create a free
   account).
2. Create a new repository (any name, e.g. `form-tracking-poc`) and
   upload the contents of this `FormTrackingPoc` folder to it — you can
   drag-and-drop the files directly in the GitHub web UI, no git command
   line needed.
3. On the repository page, click the green **Code** button → **Codespaces**
   tab → **Create codespace on main**.
4. Wait for it to spin up (it uses the `.devcontainer` config included
   here, so the .NET SDK is ready automatically). You'll get a full
   VS Code environment in your browser.
5. In the Codespaces terminal, run:
   ```bash
   dotnet run
   ```
6. A notification should pop up saying a port was forwarded, or check
   the **Ports** tab at the bottom of the VS Code window. Find port
   **5000**.
7. Right-click port 5000 → **Port Visibility** → set to **Public**.
8. Copy the forwarded URL — it'll look like
   `https://<something>-5000.app.github.dev`. This is your real HTTPS
   domain for D365 to allow-list, and it stays live as long as the
   Codespace is running.

Skip to **Step 3 — Allow the domain in D365** below, using this URL
instead of an ngrok URL.

---

## Option B — Local run + ngrok (if you're able to install software)

### Step 1 — Run the app locally

```bash
cd FormTrackingPoc
dotnet run
```

You should see it start on `http://localhost:5000`. Open that URL in a
browser and confirm you see the placeholder page with the dashed box.

### Step 2 — Expose it with ngrok

In a separate terminal:

```bash
ngrok http 5000
```

ngrok will print a public URL that looks like:

```
https://abcd1234.ngrok-free.app -> http://localhost:5000
```

Keep this terminal open — the tunnel only stays alive while ngrok is
running. Copy the `https://...ngrok-free.app` URL.

---

## Step 3 — Allow the domain in D365

In D365 Customer Insights - Journeys:

1. Go to your form record → make sure it's published/live.
2. Open the **Form hosting** tab → **Related marketing form pages** →
   add a form page if you haven't already.
3. Add your domain — either the Codespaces domain (e.g.
   `abcd1234-5000.app.github.dev`) or the ngrok domain (e.g.
   `abcd1234.ngrok-free.app`) — to the allowed domains list for
   external form hosting
   (typically under **Settings → Marketing settings / Email marketing →
   Domains**, depending on your version).
   - Full domain authentication (DNS verification) is **not** required
     for this — that's only needed if you want prefilled forms.

## Step 4 — Get the embed code

Back on the **Form hosting** tab, open your form page and grab the embed
code. You'll be offered two formats:

- **Script** — the form becomes part of the page's own layout/styles,
  and this method also tracks clicks elsewhere on the page.
- **iFrame** — the form stays visually isolated; it tracks the form
  itself but not surrounding page clicks.

Pick whichever one you want to test first.

## Step 5 — Paste the code into the POC page

Open `wwwroot/index.html` and paste the embed snippet inside the
`<div id="form-slot">` block, replacing the placeholder paragraph. Save
the file — `dotnet run` will need a restart to pick up static file
changes if you're not using `dotnet watch`.

## Step 6 — Test it

1. Open the **public URL** (Codespaces or ngrok — not localhost) in a
   browser — this is the "visit."
2. **Do not submit the form.**
3. In D365, check:
   - The contact's **Insights** tab (if this is a known/identified
     visitor), or
   - The form's own analytics for a "visited" count
4. Reload the ngrok URL a couple more times to generate a few more
   visits, then finally submit the form once and confirm you now also
   see a separate "submitted" event.

## Notes

- If nothing shows up at all, double check:
  - The domain was actually saved in the allowed-domains list (typos
    are the #1 cause of "form doesn't render").
  - Cookies aren't blocked in your test browser — tracking relies on a
    cookie being set.
  - If you're testing as a *known* contact, their tracking consent
    (Contact Point Consent, Tracking purpose) is set appropriately for
    the compliance profile's enforcement model.
- This POC intentionally does nothing clever — it's meant to isolate
  "does D365 track a bare page visit" from every other variable in a
  real production page.
