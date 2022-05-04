redirectToCheckout = function (sessionId) {
    var stripe = Stripe("pk_test_51KvdKSFrL90M42mmvdX16eT8lKbY9GfGlMbopO0NoeOiLzGPr3S6wsQawiQLJhxnLdWXKUz8ASGVlPfHVy0G4Omn00H6KQzmoo")
    stripe.redirectToCheckout({ sessionId: sessionId });
}