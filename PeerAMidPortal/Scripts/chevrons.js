function showChevrons(id) {
    var parent = document.getElementById('chevronParent');
    parent.setAttribute("style", "display:block;");
    var before = true;
    var p = null;
    for (var i = 0; i < parent.childNodes.length; ++i) {
        p = parent.childNodes[i];
        if (p.nodeName == "UL")
            break;
    }
    for (var i = 0; i < p.childNodes.length; ++i) {
        var chevron =  p.childNodes[i];
        if ((chevron.id === undefined) || (chevron.id == ''))
            continue;
        //console.log(chevron.id);
        var child = chevron.childNodes[1];
        if (chevron.id == id) {
            child.className = "innovation_tracker_history_nav-link nav-link active-color nav-link1";
            before = false;
        } else if (before) {
            child.className = "innovation_tracker_history_nav-link nav-link before-active nav-link1";
        } else {
            child.className = "innovation_tracker_history_nav-link nav-link after-active nav-link1";
        }
    }
}