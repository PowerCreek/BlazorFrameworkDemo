window.modifyStyleOf = function(label, remove, styles){
    let element = document.getElementById(label);
    if(element === null){
        return;
    }
    
    if(remove)
        element.style = null;
    
    for (let s of styles) {
        element.style.setProperty([s[0]], s[1]);
    }
}

window.modifyAttributeOf = function(label, attr, val){
    let element = document.getElementById(label);
    if(element === null){
        return;
    }

    element.setAttribute(attr, val)
}