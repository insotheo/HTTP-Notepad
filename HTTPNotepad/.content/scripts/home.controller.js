let notes = [];
let currentIndex = null;

window.onclick = function(event) {
    const contextMenu = document.getElementById('accountContextMenu');
    if (event.target !== contextMenu && !contextMenu.contains(event.target)) {
        contextMenu.style.display = 'none';
    }
};

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
    notes.forEach((note, index) => {
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
    notes.splice(index, 1);
    renderNotes();
}

function editNote(index){
    currentIndex = index;
    const note = notes[index];
    document.getElementById('noteTitle').value = note.title;
    document.getElementById('noteContent').value = note.content;
    document.getElementById('flowPanel').style.display = 'block';
    document.getElementById('overlay').style.display = 'block';
}

function saveNote(){
    const title = document.getElementById("noteTitle").value;
    const content = document.getElementById("noteContent").value;

    if(currentIndex != null){
        notes[currentIndex] = {title, content};
    } else{
        notes.push({title, content});
    }

    closeFlowPanel();
    renderNotes();
}

function addNote(){
    const note = {title: "New title", content: ""};
    notes.push(note);
    renderNotes();
}