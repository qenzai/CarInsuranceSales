This is a Telegram bot built with C# that uses the Mindee OCR API to extract data from passport and vehicle registration photos. The bot guides the user through uploading documents, confirming extracted data, agreeing to insurance pricing, and finally issues an insurance policy summary.

-------------------------------------------

Description of the bot's work
The user sends a photo of a passport.

The bot recognizes the data via Mindee OCR and sends it to the user for confirmation.

If the user does not confirm, it asks to repeat the photo.

If it confirms, it asks to send a photo of the technical passport.

Similarly, it recognizes, shows the data and waits for confirmation.

Offers a fixed insurance price (100 USD).

Waits for the user's consent.

If the user agrees, it issues an insurance policy.

If not, it apologizes and ends the session.

-------------------------------------------

