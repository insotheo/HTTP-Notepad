document.getElementById("authForm").addEventListener("submit", async function(event) {
    event.preventDefault();
    
    const isLogIn = authForm.querySelector('button').innerText === "Log In";

    if(isLogIn){
        const username = document.getElementById('username').value;
        const password = document.getElementById('password').value;

        const url = new URL("/login", window.location.origin);
        url.searchParams.append("username", username);
        url.searchParams.append("password", password);

        try{
            const response = await fetch(url, {
                method: "GET",
            });

            if(response.ok){
                const res = await response.json();
                console.log("registration success: ", res);
            }
            else{
                const res = await response.text();
                console.error("Registration failed: ", res);
                alert(res);
            }
        }
        catch(error){
            console.error("Error during login: ", error);
        }
    }
    else{
        const username = document.getElementById('username').value;
        const name = document.getElementById('name').value;
        const password = document.getElementById('password').value;
        const repeatedPassword = document.getElementById('repeatedPassword').value;

        if(password != repeatedPassword){
            alert("Repeated password must be the same as password!");
            console.error("Repeated password must be the same as password!");
            return;
        }

        const data = {
            username: username,
            name: name,
            password: password
        };

        try{
            const response = await fetch("/register", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(data),
            });

            if(response.ok){
                const res = await response.text();
                console.log("registration success: ", res);
            }
            else{
                const res = await response.text();
                console.error("Registration failed: ", res);
                alert(res);
            }
        }
        catch(error){
            console.error("Error during registration: ", error);
        }
    }
});