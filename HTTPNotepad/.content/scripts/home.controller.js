let currentIndex = null;
let user = {username: "", name: "", notes: []};

window.onload = async function(event){
    const data = JSON.parse(getCookie("MN_USER_INIT_DATA"));

    if(data == null){
        location.replace("/welcome");
    }

    user.username = data.username;
    user.name = data.name;

    document.getElementById("name").innerHTML = user.name;

    const url = new URL("/get_data", window.location.origin);
    url.searchParams.append("username", user.username);
    url.searchParams.append("name", user.name);

    try{
        const response = await fetch(url, {
            method: "GET"
        });
        
        if(response.ok){
            const res = await response.json();
            user.notes = JSON.parse(res);
            console.log("Getting data success!");
        } else{
            const res = await response.text();
            console.error("Registration failed: ", res);
        }

    } catch(error){
        console.error("Error during getting user data: ", error);
    }
}

window.onclick = function(event) {
    const contextMenu = document.getElementById('accountContextMenu');
    if (event.target !== contextMenu && !contextMenu.contains(event.target)) {
        contextMenu.style.display = 'none';
    }
};

function logout(){
    deleteCookie("MN_USER_INIT_DATA");
    location.replace("/welcome");
}

async function deleteAccount(){
    if(confirm("Are you sure, you want to delete your account?")){
        const password = prompt("Please, enter your password:");

        const url = new URL("/delete_account", window.location.origin);
        url.searchParams.append("username", user.username);
        url.searchParams.append("password", password);

        try{
            const response = await fetch(url, {
                method: "DELETE",
            });

            if(response.ok){
                logout();
            }
            else{
                const res = await response.text();
                console.error("Deleting failed: ", res);
                alert(res);
            }

        } catch(error){
            console.error("Failed deleting account: ", error);
        }
    }
}

function toggleAccountActionsMenu(event){
    event.stopPropagation();
    const contextMenu = document.getElementById('accountContextMenu');
    contextMenu.style.display = contextMenu.style.display === 'block' ? 'none' : 'block';
}

function closeFlowPanel(){
    document.getElementById('flowPanel').style.display = 'none';
    document.getElementById('overlay').style.display = 'none';
    currentNoteIndex = null;
    document.getElementById('noteTitle').value = '';
    document.getElementById('noteContent').value = '';
}

function renderNotes(){
    const notesList = document.getElementById("notesList");
    notesList.innerHTML = "";
    const isMobile = window.innerWidth < 768;
    const wordLimit = isMobile ? 3 : 10;
    user.notes.forEach((note, index) => {
        const noteDiv = document.createElement("div");
        noteDiv.className = "note";
        const previewContent = note.content.split(' ').slice(0, wordLimit).join(' ') + (note.content.split(' ').length > wordLimit ? '...' : '');
        noteDiv.innerHTML = `
                <div>
                    <strong>${note.title}</strong>
                    <p>${previewContent}</p>
                </div>
                <div>
                    <button onclick="editNote(${index})">Edit</button>
                    <button class="removeNoteBtn" onclick="removeNote(${index})">Remove</button>
                </div>
            `;
        notesList.appendChild(noteDiv);
    });
}

function removeNote(index){
    user.notes.splice(index, 1);
    renderNotes();
}

function editNote(index){
    currentIndex = index;
    const note = user.notes[index];
    document.getElementById('noteTitle').value = note.title;
    document.getElementById('noteContent').value = note.content;
    document.getElementById('flowPanel').style.display = 'block';
    document.getElementById('overlay').style.display = 'block';
}

function saveNote(){
    const title = document.getElementById("noteTitle").value;
    const content = document.getElementById("noteContent").value;

    if(currentIndex != null){
        user.notes[currentIndex] = {title, content};
    } else{
        user.notes.push({title, content});
    }

    closeFlowPanel();
    renderNotes();
}

function addNote(){
    const note = {title: "New title", content: ""};
    user.notes.push(note);
    renderNotes();
}