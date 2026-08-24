// FormTrackingPoc
//
// A tiny static-file host whose only job is to serve wwwroot/index.html,
// which is where you paste the "Script" or "iFrame" embed snippet you get
// from a D365 Customer Insights - Journeys marketing form's "Form hosting"
// tab.
//
// This project does NOT talk to Dynamics/D365 itself. It just gives you a
// real HTTP server you can run locally and then expose to the internet
// with a tunneling tool (ngrok, Cloudflare Tunnel, etc.), so D365 has a
// real HTTPS domain to allow-list and render the form against.
//
// See README.md for the full walkthrough.

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Serve wwwroot/index.html and any other static assets you drop in wwwroot.
app.UseDefaultFiles();
app.UseStaticFiles();

// Quick connectivity check you can hit locally or through your tunnel URL
// (e.g. https://<your-subdomain>.ngrok-free.app/ping) before wiring up
// D365, just to confirm the tunnel is actually reaching this process.
app.MapGet("/ping", () => Results.Text("ok - FormTrackingPoc is reachable"));

// Print the URLs this instance is listening on so it's obvious what to
// point ngrok/localtunnel at.
app.Lifetime.ApplicationStarted.Register(() =>
{
    var addresses = app.Urls;
    Console.WriteLine();
    Console.WriteLine("=================================================");
    Console.WriteLine(" FormTrackingPoc is running.");
    foreach (var url in addresses)
    {
        Console.WriteLine($"   Local URL : {url}");
    }
    Console.WriteLine();
    Console.WriteLine(" Next steps:");
    Console.WriteLine("   1. Open the local URL above and confirm you see the placeholder page.");
    Console.WriteLine("   2. In another terminal, run: ngrok http <port>");
    Console.WriteLine("   3. Copy the https://*.ngrok-free.app URL ngrok gives you.");
    Console.WriteLine("   4. Add that domain to D365's allowed domains for external form hosting.");
    Console.WriteLine("   5. Edit wwwroot/index.html and paste your form's embed code in.");
    Console.WriteLine("   6. Reload the ngrok URL (not localhost) and check D365 Insights.");
    Console.WriteLine("=================================================");
    Console.WriteLine();
});

app.Run();
