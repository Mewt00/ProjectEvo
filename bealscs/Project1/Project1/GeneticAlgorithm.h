// Darwin.h

#pragma once

#include <cstdlib>
#include <iostream>
#include <iomanip>
#include <fstream>
#include <cmath>
#include <ctime>
#include <cstring>
#include <list>
#include <iterator>
#include <math.h>
#include <vector>
#include <algorithm>
#include "Genotype.h"

using std::ifstream;
using std::cout;
using std::cerr;
using std::setw;
using std::cin;
using std::endl;
using std::string;
using std::iterator;
using std::list;
using std::vector;

namespace Evolution {

	class GeneticAlgorithm
	{
		#define POPSIZE 100
		#define MAXGENS 30
		#define NTOOLS 7
		#define PXOVER 0.8
		#define PMUTATION 0.15

		struct {
			bool operator()(Genotype &a, Genotype &b)
			{   
			    return a.getFitness() < b.getFitness();
			}   
		} compareFitness;

		vector<Genotype> population;
		vector<Genotype> newPopulation;

		int pointLimit;
		double rangedPercent;
		double meleePercent;
		double blockPercent;
		double dashPercent;
		double enemyDPSwing;
		double enemyDPShot;

	public:
		int run();

		void crossover ( );
		void elitist ( Genotype );
		void evaluate ( );
		void initialize ( char* );
		void sortPopulation ( );
		void mutate ( );
		int randval ( int );
		void selector ( );
		void verifyAndPush( Genotype );
		void Xover ( int, int );

	
	};
}