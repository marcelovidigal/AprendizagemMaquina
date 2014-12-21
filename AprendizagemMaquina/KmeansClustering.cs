using System;
using System.IO;

namespace AprendizagemMaquina
{
	class KmeansClustering
	{
		static void Main(string[] args)
		{
			Console.WriteLine("\nInicio k-means clustering\n");

			double[][] dados = new double[10][];

			dados[0] = new double[] { 73.0, 72.6 };
			dados[1] = new double[] { 61.0, 54.4 };
			dados[2] = new double[] { 67.0, 99.9 };
			dados[3] = new double[] { 68.0, 97.3 };
			dados[4] = new double[] { 62.0, 59.0 };
			dados[5] = new double[] { 75.0, 81.6 };
			dados[6] = new double[] { 74.0, 77.1 };
			dados[7] = new double[] { 66.0, 97.3 };
			dados[8] = new double[] { 68.0, 93.3 };
			dados[9] = new double[] { 61.0, 59.0 };

			//double[][] dados = getDadosArquivo("..\\..\\AlturaPeso.txt", 10, 2, ',');

			Console.WriteLine("Dados não agrupados:\n");
			Console.WriteLine(" ID Altura (in.) Peso (kg.)");
			Console.WriteLine("---------------------------------");
			exibirDados(dados, 1, true, true);

			int numeroClusteres = 3;
			Console.WriteLine("\nDefinindo a quantidade de clusteres para " + numeroClusteres);

			Console.WriteLine("\nIniciando clusterização usando o algoritmo k-means");
			Cluster cluster = new Cluster(numeroClusteres);
			int[] clusteres = cluster.clusterizar(dados);
			Console.WriteLine("Clusterização completa\n");

			Console.WriteLine("Agrupamento final na forma interna:\n");
			exibirVetor(clusteres, true);

			Console.WriteLine("Dados por cluster:\n");
			exibirClusteres(dados, clusteres, numeroClusteres, 1);
			Console.WriteLine("\nFim do programa de clusterização k-means\n");
			Console.ReadLine();
		}

		static void exibirDados(double[][] dados, int decimais, bool indices, bool novaLinha)
		{
			for (int i = 0; i < dados.Length; ++i)
			{
				if (indices == true)
					Console.Write(i.ToString().PadLeft(3) + " ");
				
				for (int j = 0; j < dados[i].Length; ++j)
				{
					double v = dados[i][j];
					Console.Write(v.ToString("F" + decimais) + " ");
				}
				
				Console.WriteLine("");
			}
			
			if (novaLinha == true)
				Console.WriteLine("");
		}

		static void exibirVetor(int[] vetor, bool novaLinha)
		{
			for (int i = 0; i < vetor.Length; ++i)
				Console.Write(vetor[i] + " ");
			
			if (novaLinha == true)
				Console.WriteLine("\n");
		}

		static void exibirClusteres(double[][] dados, int[] clusteres, int numeroClusteres, int decimais)
		{
			for (int i = 0; i < numeroClusteres; ++i)
			{
				Console.WriteLine("===================");
				
				for (int j = 0; j < dados.Length; ++j)
				{
					int clusterID = clusteres[j];
					
					if (clusterID != i)
						continue;
						
					Console.Write(j.ToString().PadLeft(3) + " ");
					
					for (int k = 0; k < dados[j].Length; ++k)
					{
						double v = dados[j][k];
						Console.Write(v.ToString("F" + decimais) + " ");
					} 
					
					Console.WriteLine("");
				} 
				
				Console.WriteLine("===================");
			} // i
		}

		static double[][] getDadosArquivo(string arquivo, int numeroLinhas, int numeroColunas, char delimitador)
		{
			FileStream fileStream = new FileStream(arquivo, FileMode.Open);
			StreamReader streamReader = new StreamReader(fileStream);
			
			string linha = "";
			string[] tokens = null; 
			int i = 0;
			double[][] resultado = new double[numeroLinhas][];

			while ((linha = streamReader.ReadLine()) != null)
			{ 
				resultado[i] = new double[numeroColunas];
				tokens = linha.Split(delimitador);
				
				for (int j = 0; j < numeroColunas; ++j)
					resultado[i][j] = double.Parse(tokens[j]);
					++i; 
			}

			streamReader.Close();
			fileStream.Close();
			
			return resultado;
		}
	}
	
	public class Cluster
	{
		private int numeroClusteres;
		private int[] clusteres;
		private double[][] centroides;
		private Random random;
		
		public Cluster(int numeroClusteres)
		{
			this.numeroClusteres = numeroClusteres;
			this.centroides = new double[numeroClusteres][];
			this.random = new Random(0); // atribuicao arbitraria
		}

		public int[] clusterizar(double[][] dados)
		{
			int numeroLinhas = dados.Length;
			int numeroColunas = dados[0].Length;
			this.clusteres = new int[numeroLinhas];

			for (int i = 0; i < numeroClusteres; ++i) // aloca cada centroide
				this.centroides[i] = new double[numeroColunas];
				
			inicializarAleatoriamente(dados);
			
			Console.WriteLine("\nClusteres inicializados aleatoriamente:");

			for (int i = 0; i < clusteres.Length; ++i)
				Console.Write(clusteres[i] + " ");

			Console.WriteLine("\n");

			bool houveMudanca = true; // houve mudanca nos clusteres?
			int maximoIteracoes = numeroLinhas * 10; // verificacao de sanidade
			int iteracao = 0;

			while (houveMudanca == true && iteracao < maximoIteracoes)
			{
				++iteracao;
				atualizarCentroides(dados);
				houveMudanca = atualizarClusteres(dados);
			}

			int[] resultado = new int[numeroLinhas];
			
			Array.Copy(this.clusteres, resultado, clusteres.Length);
			
			return resultado;
		}

		private void inicializarAleatoriamente(double[][] dados)
		{
			int numeroLinhas = dados.Length;
			int clusterID = 0;

			for (int i = 0; i < numeroLinhas; ++i)
			{
				clusteres[i] = clusterID++;
				
				if (clusterID == numeroClusteres)
					clusterID = 0;
			}
			
			for (int i = 0; i < numeroLinhas; ++i)
			{
				int r = random.Next(i, clusteres.Length); // escolhe uma celula
				int tmp = clusteres[r]; // obtem o valor da celula
				clusteres[r] = clusteres[i]; // troca os valores
				clusteres[i] = tmp;
			}
		} // inicializarAleatoriamente

		private void atualizarCentroides(double[][] dados)
		{
			int[] totalPorCluster = new int[numeroClusteres];
			
			for (int i = 0; i < dados.Length; ++i)
			{
				int clusterID = clusteres[i];
				++totalPorCluster[clusterID];
			}

			for (int i = 0; i < centroides.Length; ++i)
				for (int j = 0; j < centroides[i].Length; ++j)
					centroides[i][j] = 0.0;

			for (int i = 0; i < dados.Length; ++i)
			{
				int clusterID = clusteres[i];
				
				for (int j = 0; j < dados[i].Length; ++j)
					centroides[clusterID][j] += dados[i][j]; // soma acumulada
			}

			for (int i = 0; i < centroides.Length; ++i)
				for (int j = 0; j < centroides[i].Length; ++j)
					centroides[i][j] /= totalPorCluster[i]; // cuidado?
		} // atualizarCentroids
		
		private bool atualizarClusteres(double[][] dados)
		{
			// (re)atribui cada tupla a um cluster (com centroide mais proximo)
			// retorna falso se nao houve mudancas nas tuplas ou
			// se a reatribuicao resultou em clusteres sem tuplas atribuidas

			bool houveMudanca = false; // houve mudanca no cluster de alguma tupla?
			int[] novosClusteres = new int[clusteres.Length]; // resultado proposto
			
			Array.Copy(clusteres, novosClusteres, clusteres.Length);
			
			double[] distancias = new double[numeroClusteres]; // distancias das tuplas para os centroides

			for (int i = 0; i < dados.Length; ++i) // percorre cada tupla
			{
				for (int j = 0; j < numeroClusteres; ++j)
					distancias[j] = calcularDistancia(dados[i], centroides[j]);

				int novoClusterID = getMenorDistancia(distancias); // centroid mais proximo

				if (novoClusterID != novosClusteres[i])
				{
					houveMudanca = true; // identificado um novo cluster
					novosClusteres[i] = novoClusterID; // aceita a atualizacao
				}
			}

			if (houveMudanca == false)
				return false; // nenhuma mudanca identificada

			int[] totalPorCluster = new int[numeroClusteres];

			for (int i = 0; i < dados.Length; ++i)
			{
				int clusterID = novosClusteres[i];
				++totalPorCluster[clusterID];
			}

			for (int i = 0; i < numeroClusteres; ++i)
				if (totalPorCluster[i] == 0)
					return false; // clusterizacao ruim encontrada

			// opcional: distribui dados aleatoriamente ao cluster vazio
			for (int i = 0; i < numeroClusteres; ++i)
			{
				if (totalPorCluster[i] == 0) // cluster i nao possui itens
				{
					for (int j = 0; j < dados.Length; ++j) // encontra uma tupla para colocar no cluster i
					{
						int cid = novosClusteres[j]; // cluster de j
						int total = totalPorCluster[cid]; // quantos itens existem?
						if (total >= 2) // j esta em um cluster com 2 ou mais itens
						{
							novosClusteres[j] = i; // coloca j no cluster i
							++totalPorCluster[i]; // i agora possui um item de dados
							--totalPorCluster[cid]; // cluster que possuia t
							
							break; // verifica o proximo cluster
						}
					} // j
				} // total por cluster igual a 0
			} // i
			// teste
			Array.Copy(novosClusteres, this.clusteres, novosClusteres.Length);

			return true; // clusterizacao funcionou e houve mudanca

		} // atualizarClusteres

		private static double calcularDistancia(double[] linha, double[] centroide)
		{
			// calculo da distancia euclidiana entre dois vetores para atualizarClusteres()
			double somaQuadDif = 0.0;

			for (int i = 0; i < linha.Length; ++i)
				somaQuadDif += (linha[i] - centroide[i]) * (linha[i] - centroide[i]);

			return Math.Sqrt(somaQuadDif);
		}

		private static int getMenorDistancia(double[] distancias)
		{
			// funcao utilitaria de atualizarClusteres() para encontrar o centride mais proximo
			int indiceMenorDistancia = 0;
			double menorDistancia = distancias[0];

			for (int i = 1; i < distancias.Length; ++i)
			{
				if (distancias[i] < menorDistancia)
				{
					menorDistancia = distancias[i];
					indiceMenorDistancia = i;
				}
			}

			return indiceMenorDistancia;
		}
	}

}