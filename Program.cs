using System;
using System.Collections.Generic;
using System.Linq;

public class Palestra
{
    public string Titulo { get; }
    public int Duracao { get; } // Duração em minutos

    public Palestra(string titulo, int duracao)
    {
        Titulo = titulo;
        Duracao = duracao;
    }

    public override string ToString()
    {
        return $"{Titulo} {Duracao}min";
    }
}

public class Trilha
{
    public List<Palestra> SessaoMatutina { get; } = new List<Palestra>();
    public List<Palestra> SessaoVespertina { get; } = new List<Palestra>();
}

public static class LeitorDePalestras
{
    public static List<Palestra> LerPalestras(List<string> entrada)
    {
        List<Palestra> palestras = new List<Palestra>();

        foreach (var linha in entrada)
        {
            try
            {
                var partes = linha.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var duracaoStr = partes[^1];
                var duracao = duracaoStr == "relâmpago" ? 5 : int.Parse(duracaoStr.Replace("min", ""));
                var titulo = string.Join(' ', partes.Take(partes.Length - 1));

                palestras.Add(new Palestra(titulo, duracao));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao processar a linha: '{linha}'. Detalhes: {ex.Message}");
            }
        }

        return palestras;
    }
}

public class Organizador
{
    private const int DuracaoSessaoMatutina = 180; // 3 horas
    private const int DuracaoSessaoVespertinaMinima = 180; // 3 horas
    private const int DuracaoSessaoVespertinaMaxima = 240; // 4 horas

    public List<Trilha> OrganizarPalestras(List<Palestra> palestras)
    {
        List<Trilha> trilhas = new List<Trilha>();
        List<Palestra> restantes = new List<Palestra>(palestras);

        while (restantes.Count > 0)
        {
            Trilha trilha = new Trilha();
            trilha.SessaoMatutina.AddRange(OrganizarSessao(ref restantes, DuracaoSessaoMatutina));
            trilha.SessaoVespertina.AddRange(OrganizarSessao(ref restantes, DuracaoSessaoVespertinaMaxima, DuracaoSessaoVespertinaMinima));

            trilhas.Add(trilha);
        }

        return trilhas;
    }

    private List<Palestra> OrganizarSessao(ref List<Palestra> palestras, int duracaoMaxima, int duracaoMinima = 0)
    {
        List<Palestra> sessao = new List<Palestra>();
        int tempoRestante = duracaoMaxima;

        for (int i = palestras.Count - 1; i >= 0; i--)
        {
            if (palestras[i].Duracao <= tempoRestante)
            {
                sessao.Add(palestras[i]);
                tempoRestante -= palestras[i].Duracao;
                palestras.RemoveAt(i);
            }
        }

        if (duracaoMinima > 0 && tempoRestante > (duracaoMaxima - duracaoMinima))
        {
            for (int i = 0; i < palestras.Count; i++)
            {
                if (palestras[i].Duracao <= tempoRestante)
                {
                    sessao.Add(palestras[i]);
                    tempoRestante -= palestras[i].Duracao;
                    palestras.RemoveAt(i);
                    break;
                }
            }
        }

        return sessao;
    }
}

public class GeradorDeHorario
{
    public void ImprimirHorario(List<Trilha> trilhas)
    {
        int numeroTrilha = 1;

        foreach (var trilha in trilhas)
        {
            Console.WriteLine($"Trilha {numeroTrilha}:");

            ImprimirSessao(trilha.SessaoMatutina, new DateTime(1, 1, 1, 9, 0, 0), "Almoço");
            ImprimirSessao(trilha.SessaoVespertina, new DateTime(1, 1, 1, 13, 0, 0), "Networking Event");

            numeroTrilha++;
        }
    }

    private void ImprimirSessao(List<Palestra> sessao, DateTime inicio, string eventoFinal)
    {
        DateTime tempo = inicio;

        foreach (var palestra in sessao)
        {
            Console.WriteLine($"{tempo:HH:mm}H {palestra}");
            tempo = tempo.AddMinutes(palestra.Duracao);
        }

        Console.WriteLine($"{tempo:HH:mm}H {eventoFinal}");
    }
}

public class Program
{
    public static void Main()
    {
        List<string> entrada = new List<string>
        {
            "Writing Fast Tests Against Enterprise .Net 60min",
            "Overdoing it in Python 45min",
            "Lua for the Masses 30min",
            ".Net Errors from Mismatched Nuget Versions 45min",
            "Common .Net Errors 45min",
            "Python for .Net Developers relâmpago",
            "Communicating Over Distance 60min",
            "Accounting-Driven Development 45min",
            "Woah 30min",
            "Sit Down and Write 30min",
            "Pair Programming vs Noise 45min",
            ".Net Magic 60min",
            ".Net Core: Why We Should Move On 60min",
            "Clojure Ate Scala (on my project) 45min",
            "Programming in the Boondocks of Seattle 30min",
            ".Net vs. Clojure for Back-End Development 30min",
            ".Net Core Legacy App Maintenance 60min",
            "A World Without HackerNews 30min",
            "User Interface CSS in .Net Apps 30min"
        };

        var palestras = LeitorDePalestras.LerPalestras(entrada);
        var organizador = new Organizador();
        var trilhas = organizador.OrganizarPalestras(palestras);
        var gerador = new GeradorDeHorario();
        gerador.ImprimirHorario(trilhas);
    }
}