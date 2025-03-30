function toggleForm(){
    const authForm = document.getElementById("authForm");
    const isLoginForm = authForm.querySelector('button').innerText === "Log In";
    if(isLoginForm){ //to sign up
        authForm.innerHTML = `
            <input type="text" id="name" placeholder="Name" maxlength="255" required>
            <input type="text" id="username" placeholder="Username(can't be changed in future)" minlength="3" maxlength="50" required>
            <input type="password" id="password" placeholder="Password" minlength="8" required>
            <input type="password" id="repeatedPassword" placeholder="Repeat password" minlength="8" required>
            <button type="submit" style="width: 100%;">Sign Up</button>`;
        document.querySelector('.toggle').innerText = "Already have an account? Log In!" 
    }
    else{ //to log in
        authForm.innerHTML = `
                <input type="text" id="username" placeholder="Username" minlength="3" maxlength="50" required>
                <input type="password" id="password" placeholder="Password" minlength="8" required>
                <button type="submit" style="width: 100%;">Log In</button>`;
        document.querySelector('.toggle').innerText = "Don't have an account? Sign Up!"
    }
}