// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

export function signIn(url, ticket, persist) {
    fetch(url, { method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify({ ticket: ticket, persist: persist })})
        .then(_ => console.log("BlazorIdentity: Signed in successfully"))
        .catch((error) => console.error("BlazorIdentity:", error));
}

export function signOut(url) {
    fetch(url, { method: "POST" })
        .then(_ => console.log("BlazorIdentity: Signed out successfully"))
        .catch((error) => console.error("BlazorIdentity:", error));
}
