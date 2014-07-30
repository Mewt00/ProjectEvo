// Darwin.h

#pragma once
#include "sim/simBase.h"

#include <cstdlib>
#include <iostream>
#include <iomanip>
#include <fstream>
#include <sstream>
#include <cmath>
#include <ctime>
#include <cstring>
#include <string>
#include <list>
#include <iterator>
#include <math.h>
#include <vector>
#include <algorithm>
#include "Genotype.h"
//#include "GeneticAlgorithm_ScriptBinding.h"

using std::ifstream;
using std::ofstream;
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

	class GeneticAlgorithm : public SimObject
	{
	private:
		typedef SimObject Parent;

	public:
		struct {
			bool operator()(Genotype &a, Genotype &b)
			{   
			    return a.getFitness() < b.getFitness();
			}   
		} compareFitness;

		vector<Genotype> population;
		vector<Genotype> newPopulation;

		int generation;

		double pointLimit;
		double rangedPercent;
		double meleePercent;
		double blockPercent;
		double dashPercent;
		double enemyDPSwing;
		double enemyDPShot;

		GeneticAlgorithm() {}
		virtual ~GeneticAlgorithm() {}

	public:
		string run( );

		void crossover ( );
		void elitist ( Genotype );
		void evaluate ( );
		void initialize ( );
		void sortPopulation ( );
		void mutate ( );
		int randval ( int );
		double enemyCreationWeight(double);
		double euclideanDifference (double, double, double, double, double, double);
		void selector ( );
		void verifyAndPush( Genotype );
		void Xover ( int, int );

		bool onAdd();
		void onRemove();

		DECLARE_CONOBJECT(GeneticAlgorithm);

	};

}