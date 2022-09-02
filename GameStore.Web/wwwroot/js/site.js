// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.


function generateKey() {
    let name = document.getElementById("Name").value;
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

function resetPage() {
    let currentPageInput = document.getElementById("CurrentPage")
    currentPageInput.value = 1
}

function nextPage() {
    let currentPageInput = document.getElementById("CurrentPage")
    let currentPage = parseInt(currentPageInput.value)
    currentPage++
    currentPageInput.value = currentPage

    executeFilterForm()
}

function previousPage() {
    let currentPageInput = document.getElementById("CurrentPage")
    let currentPage = parseInt(currentPageInput.value)
    currentPage--
    currentPageInput.value = currentPage

    executeFilterForm()
}

function executeFilterForm() {
    let filterForm = document.getElementById("FilterForm")
    filterForm.submit()
}

function addCommentReply(name, id) {
    let section = document.getElementById("StateSection")
    let sectionText = document.getElementById("StateSectionText")
    let status = document.getElementById("State")
    let parentId = document.getElementById("ParentId")

    sectionText.innerText = "You are replying to " + name
    status.value = 1 // ReplyState
    parentId.value = id

    section.hidden = false
}

function addCommentQuote(name, id) {
    let section = document.getElementById("StateSection")
    let sectionText = document.getElementById("StateSectionText")
    let status = document.getElementById("State")
    let parentId = document.getElementById("ParentId")

    sectionText.innerText = "You are quoting " + name
    status.value = 2 // QuoteState
    parentId.value = id

    section.hidden = false
}

function removeCommentReplyingStatus() {
    let section = document.getElementById("StateSection")
    let sectionText = document.getElementById("StateSectionText")
    let status = document.getElementById("State")
    let parentId = document.getElementById("ParentId")

    sectionText.innerText = "";
    status.value = "0"
    parentId.value = null

    section.hidden = true
}


function submitOrderPaymentType(type) {
    setElementValue("PaymentType", type);

    submitForm("SubmitForm")
}

function setNewPageSize(pageSize) {
    setElementValue("PageSize", pageSize)
    setElementValue("CurrentPage", 1)

    submitForm("FilterForm");
}

function setElementValue(id, value) {
    let element = document.getElementById(id)
    element.value = value
}

function submitForm(id) {
    let form = document.getElementById(id)
    form.submit()
}
