using System;
using System.IO;

class QuizGame
{
    static void Main()
    {
        Player player = new Player();
        Quiz quiz = new Quiz();

        Console.WriteLine("Добро пожаловать в викторину!");
        Console.WriteLine("1. Начать новую игру");
        Console.WriteLine("2. Загрузить игру");

        string choice = Console.ReadLine();

        if (choice == "2")
        {
            player = LoadGame();
        }

        while (player.CurrentQuestion < quiz.Questions.Length)
        {
            AskQuestion(player, quiz);
            SaveGame(player);
        }

        Console.WriteLine("Игра окончена! Ваши очки: " + player.Score);
    }

    static void AskQuestion(Player player, Quiz quiz)
    {
        Console.WriteLine(quiz.Questions[player.CurrentQuestion]);
        string answer = Console.ReadLine();

        if (answer == quiz.CorrectAnswers[player.CurrentQuestion])
        {
            player.Score++;
            Console.WriteLine("Правильно!");
        }
        else
        {
            Console.WriteLine("Неправильно.");
        }

        player.CurrentQuestion++;
    }

    static void SaveGame(Player player)
    {
        using (StreamWriter writer = new StreamWriter("savegame.txt"))
        {
            writer.WriteLine(player.Score);
            writer.WriteLine(player.CurrentQuestion);
        }
    }

    static Player LoadGame()
    {
        Player player = new Player();

        if (File.Exists("savegame.txt"))
        {
            using (StreamReader reader = new StreamReader("savegame.txt"))
            {
                player.Score = int.Parse(reader.ReadLine());
                player.CurrentQuestion = int.Parse(reader.ReadLine());
            }
        }

        return player;
    }
}

class Player
{
    public int Score { get; set; }
    public int CurrentQuestion { get; set; }
}

class Quiz
{
    public string[] Questions { get; private set; }
    public string[] CorrectAnswers { get; private set; }

    public Quiz()
    {
        LoadQuestionsAndAnswers();
    }

    private void LoadQuestionsAndAnswers()
    {
        Questions = File.ReadAllLines("questions.txt");
        CorrectAnswers = File.ReadAllLines("answers.txt");
    }
}