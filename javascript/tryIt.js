document$.subscribe(() =>
{
    document.querySelectorAll("[data-fiddle]")
        .forEach(c =>
        {
            var tryItLink = document.createElement("button");
            tryItLink.addEventListener("click", () => {
                var element = document.createElement("a");
                element.style.display = "none";
                element.href = `https://dotnetfiddle.net/${c.getAttribute("data-fiddle")}`;
                element.target = "_blank";                
                document.body.appendChild(element);
                element.click();
                document.body.removeChild(element);
            });
            tryItLink.title = "Try it out in DotNetFiddle"
            tryItLink.className = "md-try-it-out md-icon";
            
            var pre = c.querySelector("pre");
            pre.insertBefore(tryItLink, pre.firstChild);
        });
});