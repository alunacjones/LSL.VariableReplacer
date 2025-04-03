document$.subscribe(() =>
{
    document.querySelectorAll("[data-fiddle]")
        .forEach(c =>
        {
            var tryItLink = document.createElement("a");
            tryItLink.innerHTML = "Try it!";
            tryItLink.href = `https://dotnetfiddle.net/${c.getAttribute("data-fiddle")}`;
            tryItLink.target = "_blank";
            tryItLink.className = "md-button md-button--primary";
            tryItLink.style.width = "fit-content";
            tryItLink.style.fontSize = "0.7em";
            
            var code = c.querySelector("code");
            code.insertBefore(tryItLink, code.firstChild);
            console.log(c.firstChild)
        });
});