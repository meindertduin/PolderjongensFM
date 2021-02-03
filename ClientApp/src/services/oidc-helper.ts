import oidc, {UserManager} from 'oidc-client';
import {oidcSettings} from "@/config/oidc";
import {vuexOidcUtils} from "vuex-oidc";
import camelCaseToSnakeCase = vuexOidcUtils.camelCaseToSnakeCase;
