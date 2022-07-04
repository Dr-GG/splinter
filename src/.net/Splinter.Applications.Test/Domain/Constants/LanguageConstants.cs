using Splinter.Applications.Test.Domain.Languages;

namespace Splinter.Applications.Test.Domain.Constants;

public static class LanguageConstants
{
    public static readonly LanguagePack Japanese = new()
    {
        Hello = "こんにちは",
        Test = "テスト",
        Goodbye = "さようなら"
    };

    public static readonly LanguagePack Gibberish = new()
    {
        Hello = "☟︎♏︎●︎●︎□︎",
        Test = "❄︎♏︎⬧︎⧫︎",
        Goodbye = "☝︎□︎□︎♎︎♌︎⍓︎♏︎"
    };

    public static readonly LanguagePack Afrikaans = new()
    {
        Hello = "Hallo",
        Test = "Ek is 'n toets",
        Goodbye = "Totsiens"
    };

    public static readonly LanguagePack English = new()
    {
        Hello = "Hello",
        Test = "I am a test",
        Goodbye = "Goodbye"
    };

    public static readonly LanguagePack French = new()
    {
        Hello = "Bonjour",
        Test = "Je suis un test",
        Goodbye = "Au revoir"
    };

    public static readonly LanguagePack German = new()
    {
        Hello = "Hallo",
        Test = "Ich bin ein prüfung",
        Goodbye = "Auf wiedersehen"
    };

    public static readonly LanguagePack Spanish = new()
    {
        Hello = "Hola",
        Test = "Soy una prueba",
        Goodbye = "Adiós"
    };
}