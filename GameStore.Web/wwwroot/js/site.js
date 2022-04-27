// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.


function generateKey() {
    var name = document.getElementById("Name").value;
    console.log(document.getElementById("Name").value);
    fetch("key/" + name, {
        method: "POST",
        headers: {
            'Accept': 'application/json; charset=utf-8',
            'Content-Type': 'application/json;charset=UTF-8'
        }
    })
        .then(response => response.json())
        .then(json => document.getElementById("Key").value = json.key)
}
