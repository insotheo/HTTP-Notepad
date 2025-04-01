document.getElementById("authForm").addEventListener("submit", async function(event) {
    event.preventDefault();
    
    const isLogIn = authForm.querySelector('button').innerText === "Log In";

    if(isLogIn){
        //TODO...
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
            const response = await fetch("http://localhost:5050/register", {
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