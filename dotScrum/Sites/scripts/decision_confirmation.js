function confirmDeletion(message) {
    var decision = document.createElement("INPUT");
    decision.type = "hidden";
    decision.name = "decision";
    if (confirm(message)) {
        decision.value = "Yes";
    } else {
        decision.value = "No";
    }

    document.forms[0].appendChild(decision);
}

function confirmDeleteTask() {
    confirmDeletion('Are you sure you want to delete the task?');
}

function confirmDeleteColumn() {
    confirmDeletion('Are you sure you want to delete the column?');
}
