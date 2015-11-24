#include "platform/platform.h"
#include "GeneticAlgorithm.h"
#include "sim/simBase.h"
#include "console/consoleTypes.h"
#include "GeneticAlgorithm_ScriptBinding.h"
#include <iostream>
#include <cmath>

using namespace Evolution;

IMPLEMENT_CONOBJECT(GeneticAlgorithm);

const int POPSIZE = 100;
const int MAXGENS = 30;
const int NTOOLS = 7;
const double PXOVER = 0.8;
const double PMUTATION = 0.15;
const string PREVIOUSROOMINFO = ".\\utilities\\ga_input.txt";
const string PASTCASEINFO = ".\\caselist.txt";
const double WEIGHTS[NTOOLS] = {1.25, 1.25, 1, 1, 1, 1, 1};

ConsoleMethod(GeneticAlgorithm, run, void, 2, 2, "() Gets the object's position.\n"
                                                              "@return chromosome.")
{
	// Calculate result.  
   object->run();
}

ConsoleMethod(GeneticAlgorithm, retrieveChromosome, const char *, 2, 2, "() Gets the object's position.\n"
                                                              "@return chromosome.")
{
	// Fetch result.  
    string result = object->retrieveChromosome();
	char* pBuffer = Con::getReturnBuffer(result.size() + 1);

	//Con::printf("%d  %d" , (result.size(),sizeof(string)));
    // Create Returnable Buffer.  
	//char* pBuffer = Con::getReturnBuffer(result.c_str());  

	dSprintf(pBuffer, result.size() + 1, "%s", result.c_str());  
	
	return pBuffer; 
}

bool GeneticAlgorithm::onAdd()
{
  if (!Parent::onAdd())
    return false;

  return true;
}

void GeneticAlgorithm::onRemove()
{
  Parent::onRemove();
}

void GeneticAlgorithm::run ( )
{
	myThread = new thread(&GeneticAlgorithm::runCalculations, this);
	//myThread->join();
}

void GeneticAlgorithm::runCalculations ( )
{
	//Con::printf("Run");
	ofstream fileOut;
	fileOut.open(".\\utilities\\enginelogfinal.txt" );
	
	vector<int>::iterator geneIterator;

	initialize ( );

	evaluate ( );

	sortPopulation() ;

	for (generation = 0; generation < MAXGENS; generation++ )
	{

		fileOut<< "Generation:  " << generation << "\n";
		fileOut<< " Population    " << population.size() << "\n";

		for(int j = 0; j < POPSIZE; ++j)
		{
			fileOut << "var("  ") = " << "\n";

			for(geneIterator = population[j].getGene()->begin(); geneIterator != population[j].getGene()->end(); ++geneIterator)
			{
				fileOut << *geneIterator << "  ";

			}

			fileOut << "\n";

			fileOut << population[j].getFitness() << "\n";

		}

		crossover ( );

		mutate ( );

		//verify();
		evaluate ( );
		selector ( );
		//elitist ( );
		fileOut<< "Generation:  " << generation << "\n";
		fileOut<< " Population    " << population.size() << "\n";
	}



	fileOut << "\n";
	fileOut << rangedPercent << "\n" << meleePercent << "\n" << blockPercent << "\n" << dashPercent << "\n" << enemyDPSwing 
		<< "\n" << enemyDPShot << "\n";


	ofstream caseList;
	caseList.open(PASTCASEINFO.c_str(), ofstream::app);

	caseList << pointLimit << " " << rangedPercent << " " << meleePercent << " " << blockPercent << " " << dashPercent << " " << enemyDPSwing << " " << enemyDPShot << " start:";

	for(geneIterator = population[population.size()-1].getGene()->begin();
		geneIterator != population[population.size()-1].getGene()->end() - 1;
		++geneIterator)
	{
			caseList << *geneIterator << " ";
	}
	caseList << *geneIterator << endl;

	caseList.close();

	for(int j = 0; j < POPSIZE; ++j)
	{
		fileOut << "var("  ") = " << "\n";

		for(geneIterator = population[j].getGene()->begin(); geneIterator != population[j].getGene()->end(); ++geneIterator)
		{
			fileOut << *geneIterator << "  ";

		}

		fileOut << "\n";

		fileOut << population[j].getFitness() << "\n";

	}

	fileOut << "\n";
	fileOut << "\n";
	fileOut << "Best fitness = " << population[population.size()-1].getFitness() << "\n";

	
	string result = "";
	char numstr[21];

	for(geneIterator = population[population.size()-1].getGene()->begin(); 
		geneIterator != population[population.size()-1].getGene()->end() - 1; 
		++geneIterator)
		{
			fileOut<< *geneIterator << " ";
			sprintf_s(numstr,"%d", *geneIterator);
			result += numstr;		
			//fileOut<< result.c_str();
			result += ' ';
		}
	sprintf_s(numstr,"%d", *(geneIterator));
	result+= numstr;

	//fileOut<< endl << result.size() << "Size" << sizeof(string) << endl;

	//Con::printf("GA %s\n",result.c_str());
	//Con::printf("Here");

	
	fileOut << endl << endl << result.c_str();
	fileOut.close();

	//return result;
	resultChromosome = result;
}

string GeneticAlgorithm::retrieveChromosome ( )
{
	myThread->join();
	return resultChromosome;
}

void GeneticAlgorithm::crossover ( )
{
	int one = 0;
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
			
			advance(geneIterator,1);
			tools[1] += *geneIterator;
			
			advance(geneIterator,1);
			tools[2] += *geneIterator;
			
			advance(geneIterator,1);
			tools[3] += *geneIterator;
			
			advance(geneIterator,1);
			tools[4] += *geneIterator;
			
			advance(geneIterator,1);
			tools[5] += *geneIterator;
			
			advance(geneIterator,1);
			tools[6] += *geneIterator;
		}
		int toolsSum = tools[0] + tools[1] + tools[2] + tools[3] + tools[4] + tools[5] + tools[6];

		population[member].setFitness( 
			( rangedPercent * tools[0] ) + ( meleePercent * tools[1] ) + ( blockPercent * tools[2] ) + ( dashPercent * tools[3] ) 
			+ ((enemyDPSwing + meleePercent)/2 * tools[4] ) + ( (enemyDPShot + rangedPercent)/2 * tools[5] )  
			+ (enemyCreationWeight(pointLimit) * tools[6]) + (enemyCreationWeight(pointLimit) * toolsSum/NTOOLS));
	}
	return;
}

void GeneticAlgorithm::initialize (  )
{
	ifstream previousRoom;
	ifstream pastCases;

	//shield parry acid tar blade projectile blob;

	srand (time(0));
	//Warning

	ofstream fileOut;
	fileOut.open(".\\utilities\\engineloginit.txt");

	previousRoom.open ( PREVIOUSROOMINFO.c_str() );
	pastCases.open(PASTCASEINFO.c_str());

	if ( !previousRoom )
	{
		cerr << "\n";
		cerr << "Initialize - Fatal error!\n";
		cerr << "  Cannot open the input file!\n";
		exit ( 1 );
	}
	// 
	//  Initialize variables within the bounds 
	//

	population.clear();
	newPopulation.clear();

	//population.resize(POPSIZE);
	newPopulation.resize(POPSIZE);

	int current = 0;

	double tempSwing, tempShot;

	previousRoom >> pointLimit >> rangedPercent >> meleePercent >> blockPercent >> dashPercent >> tempSwing >> tempShot;
	if(tempShot == 0 && tempSwing == 0)
	{
		enemyDPShot = 0;
		enemyDPSwing = 0;
	}
	else
	{
		enemyDPShot = tempShot/(tempShot + tempSwing);
		enemyDPSwing = tempSwing/(tempShot + tempSwing);
	}

fileOut << "Current Room \n" << pointLimit << endl << rangedPercent << endl << meleePercent << endl << blockPercent << endl << dashPercent << endl <<
enemyDPSwing << endl << enemyDPShot << endl;
/*Con::printf("Current Room");
Con::printf("Point %d", pointLimit);
Con::printf("Ranged %f",rangedPercent);
Con::printf("Melee %f",meleePercent);
Con::printf("Block %f",blockPercent);
Con::printf("Dash %f",dashPercent);
Con::printf("Swing %f",enemyDPSwing);
Con::printf("Shot %f",enemyDPShot);*/

Genotype genotype;
population.push_back(genotype);

previousRoom >> current;
while (!previousRoom.eof() && current >= 0)
{
fileOut << current << " ";
population[0].genePushBack(current);
previousRoom >> current;
}

fileOut<< endl;

	for(int i = 0; i < population[0].getGene()->size(); ++i)
	{
		fileOut << population[0].getGene()->at(i) << " ";
	}

int tempPoint;
double tempRanged, tempMelee, tempBlock, tempDash, tempDPSwing, tempDPShot;
double difference;
int chromosomeNum = 1;
string currentCase;


	while (std::getline(pastCases,currentCase) && chromosomeNum < POPSIZE-1)
	{
		string token = "";
		size_t pos = 0;
		pos = currentCase.find(' ',0);
		token = currentCase.substr(0, pos);
		tempPoint = std::atoi(token.c_str());
		currentCase.erase(0, pos + 1);

		if(pointLimit >= tempPoint)
		{
			std::stringstream c(currentCase);
			c >> tempRanged >> tempMelee >> tempBlock >> tempDash >> tempSwing >> tempShot;

			fileOut << "Current Case \n" << tempPoint << "\n" << tempRanged << "\n" << tempMelee << "\n" 
			<< tempBlock << "\n" << tempDash << "\n" << tempSwing << "\n" << tempShot << "\n";
	/*Con::printf("Current Case");
	Con::printf("Point %d", tempPoint);
	Con::printf("Ranged %f",tempRanged);
	Con::printf("Melee %f",tempMelee);
	Con::printf("Block %f",tempBlock);
	Con::printf("Dash %f",tempDash);
	Con::printf("Swing %f",tempSwing);
	Con::printf("Shot %f",tempShot);*/

	if(tempShot == 0 && tempSwing == 0)
	{
	tempDPShot = 0;
	tempDPSwing = 0;
	}
	else
	{
	tempDPShot = tempShot/(tempShot + tempSwing);
	tempDPSwing = tempSwing/(tempShot + tempSwing);
	}

	difference = euclideanDifference(tempRanged, tempMelee, tempBlock, tempDash, tempDPSwing, tempDPShot);

	fileOut<< "Difference:" << difference << "\n";
	//Con::printf("diff: %f", difference);

	if(difference < .75 )
	{
		population.push_back(genotype);
		//Con::printf("Inside \n");
		//fileOut<< "Inside" << endl;
		std::getline(c, currentCase);
		//string delimiter = " ";
		//size_t pos = 0;
		//string token = "";
		//fileOut<< currentCase << endl;
		currentCase = currentCase.erase(0,7);
		//fileOut<< currentCase << endl;
		while ((pos = currentCase.find(' ',0)) != string::npos) 
		{
			//Con::printf("Pos %d \n", pos);
			//fileOut << "Pos " << pos << endl;
			//fileOut << "substr " << currentCase.substr(0, pos) << endl;
			token = currentCase.substr(0, pos);
			//Con::printf("Token %s \n", token.c_str());
			//fileOut << "Token " << token << endl;
			//fileOut << "Token atoi " << atoi(token.c_str()) << endl;
			population[chromosomeNum].genePushBack(std::atoi(token.c_str()));
			//Con::printf("Token %d \n", std::atoi(token.c_str()));
			currentCase.erase(0, pos + 1);
			//Con::printf("Case %s \n", currentCase.c_str());
		}
		population[chromosomeNum].genePushBack(std::atoi(currentCase.c_str()));
		//Con::printf("Case after while %s \n", currentCase.c_str());

		/*while (!previousRoom.eof() && pastCases.peek() != '\n')
		{
		pastCases >> current;
		fileOut << current << " ";
		population[chromosomeNum].genePushBack(current);
		}
		fileOut<< "\n";*/
		++chromosomeNum;
	}
}
}

pastCases.close();

while(chromosomeNum < POPSIZE)
{
population.push_back(genotype);
//double points = 0;
population[chromosomeNum].setGene(*population[0].getGene());

/*for(geneIterator = population[0].getGene()->rbegin(); geneIterator != population[0].getGene()->rend(); ++geneIterator)
{

population[j].genePushBack(randval(*geneIterator));

}*/
++chromosomeNum;

}

vector<int>::iterator geneIterator;
chromosomeNum = 0;
while(chromosomeNum < POPSIZE)
{
//int currentTool = 0;
fileOut<< "Gene " << chromosomeNum << " ---> ";
for(geneIterator = population[chromosomeNum].getGene()->begin(); 
geneIterator != population[chromosomeNum].getGene()->end(); ++geneIterator)
{
//++currentTool;

//points += *geneIterator * WEIGHTS[currentTool%NTOOLS];
fileOut<< *geneIterator << " ";

}
++chromosomeNum;
fileOut<< endl;
}

previousRoom.close ( );
fileOut.close();

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
	int currentPopulation = population.size();

	for (int i = 0; i < currentPopulation; i++ )
	{
		transfer = 0;
		Genotype g;
		g.setGene(*population[i].getGene());
		for (geneIterator = g.getGene()->begin(); geneIterator != g.getGene()->end(); ++geneIterator)
		{

			x = rand ( ) % 1000 / 1000.0;
///////////////////
// finds two values for tools and increments or decrements them
///////////////////
			first = 0;
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
					first = 0;
				}
			}
		}

		if(transfer > 1)
			verifyAndPush(g);

	}

	return;
}

int GeneticAlgorithm::randval ( int x )
{
	int val;

	val = ( ( rand() % 1000 ) / 1000.0 ) * 2 + (std::abs(x - 1));			//Warning

	return val;
}

double GeneticAlgorithm::enemyCreationWeight(double d )
{
	if(d < 100)
		return (2/M_PI) * atan(d/150);
	else
		return d/(125*M_PI);
}

double GeneticAlgorithm::euclideanDifference (double r, double m, double b, double d, double sw, double sh)
{
	double diff = 0;

	if(r != 0 || rangedPercent != 0)
		diff += pow(r - rangedPercent, 2)/((r + rangedPercent)/2); 
		
	if(m != 0 || meleePercent != 0)
		diff += pow(m - meleePercent, 2)/((m + meleePercent)/2); 

	if(b != 0 || blockPercent != 0)
		diff += pow(b - blockPercent, 2)/((b + blockPercent)/2);
		
	if(d != 0 || dashPercent != 0)
		diff += pow(d - dashPercent, 2)/((d + dashPercent)/2);

	if(sw != 0 || enemyDPSwing != 0)
		diff += pow(sw - enemyDPSwing, 2)/((sw + enemyDPSwing)/2);
				
	if(sh != 0 || enemyDPShot != 0)
		diff += pow(sh - enemyDPShot, 2)/((sh + enemyDPShot)/2);

	diff = sqrt(diff);

	return diff;
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
	cout<< newPopulation.size() << "\n";
	newPopulation.clear();
	newPopulation.resize(POPSIZE);

	sortPopulation();

	return;     
}

void GeneticAlgorithm::verifyAndPush( Genotype g )
{
	vector<int>::iterator geneIterator;
	double points = 0;
	int currentTool = 0;
	int blobRec = 1;
	int toolSlots = 0;
	int toolTypes = 0;

	for(geneIterator = g.getGene()->begin(); geneIterator != g.getGene()->end(); ++geneIterator)
		{
			++currentTool;
			
			//On blob count
			if(currentTool%NTOOLS > 0)
			{
				if(*geneIterator > 0)
				{
					++toolTypes;
					//++toolSlots;
					//if(*geneIterator/5 > 0)
					//	toolSlots += *geneIterator/5;

				}

			}
			else
			{
				blobRec += toolTypes/4;

				if(*geneIterator < blobRec)
					*geneIterator = blobRec;

				if(toolTypes > 0)
					points += (--toolTypes) * 2;
				
				blobRec = 1;
				toolTypes = 0;
				//toolSlots = 0;
			}
			points += *geneIterator * WEIGHTS[currentTool%NTOOLS];

			//points += *geneIterator;

		}

	//Con::printf("%f / %f mutate" , points, pointLimit);


	if(points <= pointLimit)
		population.push_back(g);



}

void GeneticAlgorithm::Xover ( int a, int b )
{
	int pointA; // parent A crossover point
	int pointB; // parent B crossover point

	vector<int>::iterator geneIterator;

	// 
	//  Select the crossover point.
	//

	//			A	  B
	//point -- [XXX[P)XX)
	//
	int p = pointLimit;
	if((p - 5) % 10 == 0 && generation == 0)
		for(int i = 0; i < NTOOLS; i++)
		{
			population[a].getGene()->insert(population[a].getGene()->end(), 0);
			population[b].getGene()->insert(population[b].getGene()->end(), 0);
		}

	pointA = rand () % ( population[a].getGene()->size() );

	//pointB = (pointA % NTOOLS) + 
	//	NTOOLS * ( rand () % ( population[b].getGene()->size() / NTOOLS) ) ;

	pointB = rand () % ( population[b].getGene()->size() );

	// 
	//  Splice the parents together 
	//

	Genotype g;
	Genotype g2;
	


	g.assignGene(population[a].spliceGene(0, pointA), population[b].spliceGene(pointB, population[b].getGene()->size()));

	if((g.getGene()->size()%NTOOLS) != 0)
	{
			string result = "";
			char numstr[21];

			for(geneIterator = g.getGene()->begin(); 
				geneIterator != g.getGene()->end() - 1; 
				++geneIterator)
				{
					sprintf_s(numstr,"%d", *geneIterator);
					result += numstr;		
					result += ' ';
				}
			sprintf_s(numstr,"%d", *(geneIterator));
			result+= numstr;
			//Con::printf("GA %s\n",result.c_str());
	}
	while((g.getGene()->size()%NTOOLS) != 0) 
	{
		g.genePushBack(0);
		//Con::printf("In loop");
		//Con::printf("%d", (g.getGene()->size()%NTOOLS));
	}

	//Con::printf("After 0 added");

	g2.assignGene(population[b].spliceGene(0, pointB), population[a].spliceGene(pointA, population[a].getGene()->size()));
	while((g2.getGene()->size()%NTOOLS) != 0)
		g2.genePushBack(0);

	verifyAndPush(g);
	verifyAndPush(g2);


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