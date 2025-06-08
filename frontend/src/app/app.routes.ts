import { Routes } from '@angular/router';

import { HomeComponent } from './pages/home/home.component';
import { ExecutionsComponent } from './pages/executions/executions.component';

import { TransformersComponent } from './pages/transformers/transformers.component';

import { CreateComponent as ClassCreateComponent } from './pages/classes/create/create.component';
import { UpdateComponent as ClassUpdateComponent } from './pages/classes/update/update.component';
import { DeleteComponent as ClassDeleteComponent } from './pages/classes/delete/delete.component';

import { CreateComponent as AttributeCreateComponent } from './pages/attributes/create/create.component';
import { UpdateComponent as AttributeUpdateComponent } from './pages/attributes/update/update.component';
import { DeleteComponent as AttributeDeleteComponent } from './pages/attributes/delete/delete.component';

import { CreateComponent as MethodCreateComponent } from './pages/methods/create/create.component';
import { DeleteComponent as MethodDeleteComponent } from './pages/methods/delete/delete.component';

import { CreateComponent as ParameterCreateComponent } from './pages/parameters/create/create.component';

import { CreateComponent as VariableCreateComponent } from './pages/variables/create/create.component';

import { CreateComponent as InvocationCreateComponent } from './pages/invocations/create/create.component';

import { CreateComponent as NamespaceCreateComponent } from './pages/namespaces/create/create.component';
import { LocalStorageGuard } from './guards/local-storage.guard';
import { ForbbidenComponent } from './pages/forbbiden/forbbiden.component';

import { TransformerGetComponent } from './pages/transformers/transformer-get/transformer-get.component';

import { PatternExamplesComponent } from './pages/pattern-examples/pattern-examples.component';
import { VisitorComponent } from './pages/pattern-examples/visitor/visitor.component';
import { StrategyComponent } from './pages/pattern-examples/strategy/strategy.component';
import { CompositeComponent } from './pages/pattern-examples/composite/composite.component';
import { TemplateMethodComponent } from './pages/pattern-examples/template-method/template-method.component';

export const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'home', component: HomeComponent },
    { path: 'forbidden', component: ForbbidenComponent },

    { path: 'execute', component: ExecutionsComponent },
    {
        path: 'transformers',
        component: TransformersComponent,
        canActivate: [LocalStorageGuard],
        data: {
            storageKey: 'authToken',
            redirectTo: '/forbidden'
        }
    },
    {
        path: 'transformers/get',
        component: TransformerGetComponent,
        canActivate: [LocalStorageGuard],
        data: {
            storageKey: 'authToken',
            redirectTo: '/forbidden'
        }
    },
    { path: 'classes/create', component: ClassCreateComponent },
    { path: 'classes/update', component: ClassUpdateComponent },
    { path: 'classes/delete', component: ClassDeleteComponent },

    { path: 'attributes/create', component: AttributeCreateComponent },
    { path: 'attributes/update', component: AttributeUpdateComponent },
    { path: 'attributes/delete', component: AttributeDeleteComponent },

    { path: 'methods/create', component: MethodCreateComponent },
    { path: 'methods/delete', component: MethodDeleteComponent },

    { path: 'parameters/create', component: ParameterCreateComponent },

    { path: 'variables/create', component: VariableCreateComponent },

    { path: 'invocations/create', component: InvocationCreateComponent },

    { path: 'namespaces/create', component: NamespaceCreateComponent },

    { path: 'patterns/prueba', component: PatternExamplesComponent },
    { path: 'patterns/visitor', component: VisitorComponent },
    { path: 'patterns/composite', component: CompositeComponent },
    { path: 'patterns/strategy', component: StrategyComponent },
    { path: 'patterns/templateMethod', component: TemplateMethodComponent },

    { path: '**', redirectTo: 'home' },

    {
        path: 'variables/:id',
        loadComponent: () => import('./pages/variables/create/create.component').then(m => m.CreateComponent)
    },

    {
        path: 'variables/create',
        loadComponent: () => import('./pages/variables/create/create.component').then(m => m.CreateComponent)
    }
];