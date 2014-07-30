#include "GeneticAlgorithm.h"

using namespace Evolution;

int GeneticAlgorithm::run ()
{

	initialize ( "ga_input.txt" );

	evaluate ( );

	sortPopulation() ;

	for (int generation = 0; generation < MAXGENS; generation++ )
	{

		crossover ( );
		mutate ( );
		//verify();
		evaluate ( );
		selector ( );
		//elitist ( );
		cout<< "Generation:  " << generation << "\n";
		cout << "Best fitness = " << population[population.size()-1].getFitness() << "\n";
		//cout<< " Population    " << population.size() << "\n";
	}

	cout << "\n";
	cout << rangedPercent << "\n" << meleePercent << "\n" << blockPercent << "\n" << dashPercent << "\n" << enemyDPSwing 
		<< "\n" << enemyDPShot << "\n";

	vector<int>::iterator geneIterator;


	for(int j = 0; j < POPSIZE; ++j)
	{
		//cout << "var("  ") = " << "\n";

		for(geneIterator = population[j].getGene()->begin(); geneIterator != population[j].getGene()->end(); ++geneIterator)
		{
			//cout << *geneIterator << "  ";

		}

		//cout << "\n";

		//cout << population[j].getFitness() << "\n";

	}

	cout << "\n";
	cout << "\n";
	cout << "Best fitness = " << population[population.size()-1].getFitness() << "\n";

	return 0;
}

void GeneticAlgorithm::crossover ( )
{
	int one;
	int numParents = 0;
	double x;

	for (int mem = 0; mem < POPSIZE; ++mem )
	{
		x = ( rand ( ) % 1000 ) / 1000.0;
		if ( x < PXOVER )
		{
			++numParents;

			if ( numParents % 2 == 0 )
				Xover (one, mem);

			else
				one = mem;
		}
	}
	return;
}

void GeneticAlgorithm::elitist ( Genotype oldBest )
{

	if(newPopulation[newPopulation.size() - 1].getFitness() < oldBest.getFitness()) 
		newPopulation[0] = oldBest;

	return;

}

void GeneticAlgorithm::evaluate ( )
{
	int tools[NTOOLS];
	vector<int>::iterator geneIterator;

	for (int member = 0; member < population.size(); ++member )
	{
		tools[0] = 0;
		tools[1] = 0;
		tools[2] = 0;
		tools[3] = 0;
		tools[4] = 0;
		tools[5] = 0;
		tools[6] = 0;
		for(geneIterator = population[member].getGene()->begin(); geneIterator != population[member].getGene()->end(); ++geneIterator)
		{

			tools[0] += *geneIterator;
			//cout << " Tool 0::  " << tools[0];
			advance(geneIterator,1);
			tools[1] += *geneIterator;
			//cout << " Tool 1::  " << tools[1];
			advance(geneIterator,1);
			tools[2] += *geneIterator;
			//cout << " Tool 2::  " << tools[2];
			advance(geneIterator,1);
			tools[3] += *geneIterator;
			//cout << " Tool 3::  " << tools[3];
			advance(geneIterator,1);
			tools[4] += *geneIterator;
			//cout << " Tool 4::  " << tools[4];
			advance(geneIterator,1);
			tools[5] += *geneIterator;
			//cout << " Tool 5::  " << tools[5];
			advance(geneIterator,1);
			tools[6] += *geneIterator;
			//cout << "  Tool 6::  " << tools[6] << "\n";
			//		if(geneIterator != population[member].gene.end())
			//			advance(geneIterator,1);
		}
		population[member].setFitness( ( rangedPercent * tools[0] ) + ( meleePercent * tools[1] ) + ( blockPercent * tools[2] ) 
			+ ( dashPercent * tools[3] ) + (enemyDPSwing/(enemyDPSwing + enemyDPShot) * tools[4] ) + ( enemyDPShot/(enemyDPSwing + enemyDPShot) * tools[5] )  );
	}
	return;
}

void GeneticAlgorithm::initialize ( char* filename  )
{
	ifstream file_in;

	//int shield;
	//int parry;
	//int acid;
	//int tar;
	//int blade;
	//int projectile;
	//int blob;

	srand (time(0));								//Warning

	file_in.open ( filename );

	if ( !file_in )
	{
		cerr << "\n";
		cerr << "Initialize - Fatal error!\n";
		cerr << "  Cannot open the input file!\n";
		exit ( 1 );
	}
	// 
	//  Initialize variables within the bounds 
	//

	population.resize(POPSIZE);
	newPopulation.resize(POPSIZE);

	int current = 0;

	file_in >> pointLimit >> rangedPercent >> meleePercent >> blockPercent >> dashPercent >> enemyDPSwing >> enemyDPShot;

	//cout<< pointLimit << endl;

	while (!file_in.eof())
	{
		file_in >> current;
		population[0].genePushBack(current);
	}

	int j = 1;
	vector<int>::reverse_iterator geneIterator;
	while(j < POPSIZE)
	{
		int points = 0;
		
		population[j].setGene(*population[0].getGene());

		/*for(geneIterator = population[0].getGene()->rbegin(); geneIterator != population[0].getGene()->rend(); ++geneIterator)
		{

			population[j].genePushBack(randval(*geneIterator));

		}*/

		for(geneIterator = population[j].getGene()->rbegin(); geneIterator != population[j].getGene()->rend(); ++geneIterator)
		{

			points += *geneIterator;

		}

		if(points <= pointLimit)
			++j;

	}

	file_in.close ( );

	return;
}

void GeneticAlgorithm::sortPopulation ( )
{

	std::sort(population.begin(), population.end(), compareFitness);

	return;
}

void GeneticAlgorithm::mutate ( )
{
	int transfer;
	int first;
	double x;
	vector<int>::iterator geneIterator;


	for (int i = 0; i < population.size(); i++ )
	{
		transfer = 0;
		Genotype g;
		g.setGene(*population[i].getGene());
		for (geneIterator = g.getGene()->begin(); geneIterator != g.getGene()->end(); ++geneIterator)
		{

			x = rand ( ) % 1000 / 1000.0;
			if ( x < PMUTATION )
			{
				++transfer;

				if ( transfer % 2 == 1 )
					first = distance(g.getGene()->begin(),geneIterator);

				else
				{
					if(g.getGene()->at(first) < *geneIterator)
					{
						++g.getGene()->at(first);
						--*geneIterator;
					}
					else if(g.getGene()->at(first) > *geneIterator)
					{
						--g.getGene()->at(first);
						++*geneIterator;
					}
					else
					{
						++g.getGene()->at(first);
						++*geneIterator;
					}
				}
			}
		}

		verifyAndPush(g);

	}

	return;
}

int GeneticAlgorithm::randval ( int x )

	//****************************************************************************80
	// 
	//  Purpose:
	//
	//    RANDVAL generates a random value within bounds.
	//
	//  Modified:
	//
	//    29 December 2007
	//
{
	int val;

	val = ( ( rand() % 1000 ) / 1000.0 ) * 2 + (std::abs(x - 1));			//Warning

	return val;
}

void GeneticAlgorithm::selector ( )
{
	int i;
	int j;
	int member;
	double p;
	double sum = 0;

	//
	//	Order the current population by fitness
	//

	std::sort(population.begin(), population.end(), compareFitness);

	//
	//  Find total fitness of the population 
	//
	for ( member = 0; member < population.size(); member++ )
	{
		sum = sum + population[member].getFitness();
	}
	//
	//  Calculate the relative fitness.
	//
	for ( member = 0; member < population.size(); member++ )
	{
		population[member].setRFitness( population[member].getFitness() / sum );
	}
	population[0].setCFitness( population[0].getRFitness() );
	// 
	//  Calculate the cumulative fitness.
	//
	for ( member = 1; member < population.size(); member++ )
	{
		population[member].setCFitness( population[member-1].getCFitness() +       
			population[member].getRFitness() );
	}
	// 
	//  Select survivors using cumulative fitness. 
	//
	for ( i = 0; i < newPopulation.size(); i++ )
	{ 
		p = rand() % 1000 / 1000.0;
		if (p < population[0].getCFitness())
		{
			newPopulation[i] = population[0];      
		}
		else
		{
			for ( j = 0; j < population.size() - 1; j++ )
			{ 
				if ( p >= population[j].getCFitness() && p < population[j+1].getCFitness() )
				{
					newPopulation[i] = population[j+1];
				}
			}
		}
	}

	std::sort(newPopulation.begin(), newPopulation.end(), compareFitness);

	elitist(population[population.size() - 1]);
	// 
	//  Once a new population is created, copy it back 
	//

	population.clear();
	population.resize(POPSIZE);
	for ( i = 0; i < POPSIZE; i++ )
	{
		population[i] = newPopulation[i]; 
	}
	//cout<< newPopulation.size() << "\n";
	newPopulation.clear();
	newPopulation.resize(POPSIZE);

	sortPopulation();

	return;     
}

void GeneticAlgorithm::verifyAndPush( Genotype g )
{
	vector<int>::iterator geneIterator;
	int points = 0;

	for(geneIterator = g.getGene()->begin(); geneIterator != g.getGene()->end(); ++geneIterator)
		{

			points += *geneIterator;

		}

	if(points <= pointLimit)
		population.push_back(g);



}

void GeneticAlgorithm::Xover ( int a, int b )
{
	int pointA; // parent A crossover point
	int pointB; // parent B crossover point

	// 
	//  Select the crossover point.
	//

	//			A	  B
	//point -- [XXX[P)XX)
	//

	pointA = rand () % ( population[a].getGene()->size() );
	//cout<< pointA << "\n";

	pointB = (pointA % NTOOLS) +
		NTOOLS * ( rand () % ( population[b].getGene()->size() / NTOOLS) ) ;
	//cout<< pointB << "\n";

	// 
	//  Splice the parents together 
	//

	Genotype g;

	g.assignGene(population[a].spliceGene(0, pointA), population[b].spliceGene(pointB, population[b].getGene()->size()));

	vector<int>::iterator geneIterator;
	int points = 0;

	for(geneIterator = g.getGene()->begin(); geneIterator != g.getGene()->end(); ++geneIterator)
		{

			points += *geneIterator;

		}

	if(points <= pointLimit)
		population.push_back(g);


	/*	Method to allow 4 parents and cut/splice

	cout<<one<<two<< point[0][0]<< "\n";
	point[0][0] = ( rand ( ) % ( population[one].getGene()->size() - 3 ) ) + 1;
	cout<< point[0][0]<< "\n";

	point[1][0] = (point [0][0] % NTOOLS) +
	NTOOLS * ( rand ( ) % ( population[two].getGene()->size() / NTOOLS) ) ;
	cout<< point[0][0]<< "\n";
	point[2][0] = (point [0][0] % NTOOLS) +
	NTOOLS * ( rand ( ) % ( population[three].getGene()->size() / NTOOLS) ) ;
	cout<< point[0][0]<< "\n";
	point[3][0] = (point [0][0] % NTOOLS) +
	NTOOLS * ( rand ( ) % ( population[four].getGene()->size() / NTOOLS) ) ;
	cout<< point[0][0]<< "\n";

	//point[0][1]
	point[0][1] = (point[0][0]) + 
	( rand ( ) % ( population[one].getGene()->size() - 2 - point[0][0]) ) + 1;
	cout<< point[0][0]<< "\n";
	//point[1][1]
	if(point[1][0] > (point[0][1] % NTOOLS) )
	point[1][1] = (point [0][1] % NTOOLS) +
	NTOOLS * ( rand ( ) % ( (population[two].getGene()->size() / NTOOLS) 
	- (point[1][0] / NTOOLS) - 1) 
	+ (point[1][0] / NTOOLS) + 1) ;
	else
	point[1][1] = (point [0][1] % NTOOLS) +
	NTOOLS * ( rand ( ) % ( (population[two].getGene()->size() / NTOOLS) 
	- (point[1][0] / NTOOLS) ) 
	+ point[1][0] / NTOOLS) ;
	cout<< point[0][0]<< "\n";
	//point[2][1]
	if(point[2][0] > (point[0][1] % NTOOLS) )
	point[2][1] = (point [0][1] % NTOOLS) +
	NTOOLS * ( rand ( ) % ( (population[three].getGene()->size() / NTOOLS) 
	- (point[2][0] / NTOOLS) - 1) 
	+ (point[2][0] / NTOOLS) + 1) ;
	else
	point[2][1] = (point [0][1] % NTOOLS) +
	NTOOLS * ( rand ( ) % ( (population[three].getGene()->size() / NTOOLS) 
	- (point[2][0] / NTOOLS) ) 
	+ point[2][0] / NTOOLS ) ;

	//point[3][1]
	if(point[3][0] > (point[0][1] % NTOOLS) )
	point[3][1] = (point [0][1] % NTOOLS) +
	NTOOLS * ( rand ( ) % ( (population[four].getGene()->size() / NTOOLS) 
	- (point[3][0] / NTOOLS) - 1) 
	+ (point[3][0] / NTOOLS) + 1) ;
	else
	point[3][1] = (point [0][1] % NTOOLS) +
	NTOOLS * ( rand ( ) % ( (population[four].getGene()->size() / NTOOLS) 
	- (point[3][0] / NTOOLS) ) + point[3][0] / NTOOLS ) ;


	//point[0][2]
	point[0][2] = (point[0][1]) +
	( rand ( ) % ( population[one].getGene()->size() - 1 ) ) + 1;
	//point[1][2]
	if(point[1][1] > (point[0][2] % NTOOLS) )
	point[1][2] = (point [0][2] % NTOOLS) +
	NTOOLS * ( rand ( ) % ( (population[two].getGene()->size() / NTOOLS) 
	- (point[1][1] / NTOOLS) - 1) + (point[1][1] / NTOOLS) + 1) ;
	else
	point[1][2] = (point [0][2] % NTOOLS) +
	NTOOLS * ( rand ( ) % ( (population[two].getGene()->size() / NTOOLS) 
	- (point[1][1] / NTOOLS) ) + point[1][1] / NTOOLS ) ;


	//point[2][2]
	if(point[2][1] > (point[0][2] % NTOOLS) )
	point[2][2] = (point [0][2] % NTOOLS) +
	NTOOLS * ( rand ( ) % ( (population[three].getGene()->size() / NTOOLS) 
	- (point[2][1] / NTOOLS) - 1) + (point[2][1] / NTOOLS) + 1) ;
	else
	point[2][2] = (point [0][2] % NTOOLS) +
	NTOOLS * ( rand ( ) % ( (population[three].getGene()->size() / NTOOLS) 
	- (point[2][1] / NTOOLS) ) + point[2][1] / NTOOLS ) ;

	//point[3][2]
	if(point[3][1] > (point[0][2] % NTOOLS) )
	point[3][2] = (point [0][2] % NTOOLS) +
	NTOOLS * ( rand ( ) % ( (population[four].getGene()->size() / NTOOLS) 
	- (point[3][1] / NTOOLS) - 1) + (point[3][1] / NTOOLS) + 1) ;
	else
	point[3][2] = (point [0][2] % NTOOLS) +
	NTOOLS * ( rand ( ) % ( (population[four].getGene()->size() / NTOOLS) 
	- (point[3][1] / NTOOLS) ) + point[3][1] / NTOOLS ) ;

	for (int i = 0; i < 4; i++ )
	{
	struct genotype g;

	g.fitness = 0;
	g.cfitness = 0;
	g.rfitness = 0;

	std::list<int>::iterator beginIterator;
	std::list<int>::iterator endIterator;

	//Part 1 of new gene
	endIterator = population[one].gene.begin();
	std::advance(endIterator, point[0][0] + 1);
	for(beginIterator = population[one].gene.begin(); 
	beginIterator != endIterator; 
	++beginIterator)
	{
	g.gene.push_back(*beginIterator);
	}

	//Part 2
	beginIterator = population[two].gene.begin();
	endIterator = population[two].gene.begin();
	std::advance(beginIterator, point[1][0]);
	std::advance(endIterator, point[1][1] + 1);
	for(; 
	beginIterator != endIterator; 
	++beginIterator)
	{
	g.gene.push_back(*beginIterator);
	}

	//Part 3
	beginIterator = population[three].gene.begin();
	endIterator = population[three].gene.begin();
	std::advance(beginIterator, point[2][1]);
	std::advance(endIterator, point[2][2] + 1);
	for(; 
	beginIterator != endIterator; 
	++beginIterator)
	{
	g.gene.push_back(*beginIterator);
	}

	//Part 4
	beginIterator = population[four].gene.begin();
	std::advance(beginIterator, point[3][2]);
	for(; 
	beginIterator != population[four].gene.end(); 
	++beginIterator)
	{
	g.gene.push_back(*beginIterator);
	}

	population.push_back(g);
	//r8_swap ( &population[one].gene[i], &population[two].gene[i] );
	}*/


	return;
}